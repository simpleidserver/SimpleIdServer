// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api.BCCallback;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public class BackChannelConsentsController : Controller
    {
        private readonly IDataProtector _dataProtector;
        private readonly IClientRepository _clientRepository;
        private readonly IdServer.Infrastructures.IHttpClientFactory _httpClientFactory;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IAmrHelper _amrHelper;
        private readonly ILogger<BackChannelConsentsController> _logger;

        public BackChannelConsentsController(
            IDataProtectionProvider dataProtectionProvider, 
            IClientRepository clientRepository, IdServer.Infrastructures.IHttpClientFactory httpClientFactory,
            IJwtBuilder jwtBuilder, IAmrHelper amrHelper, ILogger<BackChannelConsentsController> logger)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _clientRepository = clientRepository;
            _httpClientFactory = httpClientFactory;
            _jwtBuilder = jwtBuilder;
            _amrHelper = amrHelper;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            try
            {
                var queries = ExtractQuery(returnUrl);
                if (!User.Identity.IsAuthenticated)
                {
                    var amr = queries["amr"].GetValue<string>();
                    returnUrl = $"{Request.GetAbsoluteUriWithVirtualPath()}{Url.Action("Index", "BackChannelConsents")}?returnUrl={returnUrl}";
                    return RedirectToAction("Index", "Authenticate", new { area = amr, ReturnUrl = _dataProtector.Protect(returnUrl) });
                }

                var viewModel = await BuildViewModel(queries, returnUrl, cancellationToken);
                return View(viewModel);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "cryptography_error", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConfirmBCConsentsViewModel confirmConsentsViewModel, CancellationToken cancellationToken)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Errors", new { code = "unauthorized", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            if (confirmConsentsViewModel == null) 
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            var viewModel = new BCConsentsIndexViewModel
            {
                ReturnUrl = confirmConsentsViewModel.ReturnUrl
            };
            try
            {
                var issuer = $"{Request.GetAbsoluteUriWithVirtualPath()}/{Constants.EndPoints.BCCallback}";
                var queries = ExtractQuery(confirmConsentsViewModel.ReturnUrl);
                viewModel = await BuildViewModel(queries, viewModel.ReturnUrl, cancellationToken);
                var parameter = new BCCallbackParameter
                {
                    Action = confirmConsentsViewModel.IsRejected ? BCCallbackActions.REJECT : BCCallbackActions.CONFIRM,
                    AuthReqId = viewModel.AuthReqId
                };
                var sub = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Claims = new Dictionary<string, object>
                    {
                        { JwtRegisteredClaimNames.Sub, sub },
                    }
                };
                var idToken = _jwtBuilder.Sign(tokenDescriptor, SecurityAlgorithms.RsaSha256);
                using (var httpClient = _httpClientFactory.GetHttpClient())
                {
                    var json = JsonSerializer.Serialize(parameter);
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(issuer),
                        Content = new StringContent(JsonSerializer.Serialize(parameter), System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Add(Constants.AuthorizationHeaderName, $"{AutenticationSchemes.Bearer} {idToken}");
                    var responseMessage = await httpClient.SendAsync(request, cancellationToken);
                    try
                    {
                        responseMessage.EnsureSuccessStatusCode();
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                        ModelState.AddModelError("invalid_request", "cannot_confirm_or_reject");
                        return View(viewModel);
                    }
                }

                return View(viewModel);
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("invalid_request", "cryptography_error");
                return View(viewModel);
            }
        }

        private Task<BCConsentsIndexViewModel> BuildViewModel(string returnUrl, CancellationToken cancellationToken) => BuildViewModel(ExtractQuery(returnUrl), returnUrl, cancellationToken);

        private async Task<BCConsentsIndexViewModel> BuildViewModel(JsonObject queries, string returnUrl, CancellationToken cancellationToken)
        {
            var viewModel = new BCConsentsIndexViewModel
            {
                AuthReqId = queries.GetAuthReqId(),
                ClientId = queries.GetClientId(),
                BindingMessage = queries.GetBindingMessage(),
                Scopes = queries.GetScopes(),
                ReturnUrl = returnUrl
            };
            var client = await _clientRepository.Query().Include(c => c.Translations).AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == viewModel.ClientId, cancellationToken);
            viewModel.ClientName = client.ClientName;
            return viewModel;
        }

        private JsonObject ExtractQuery(string returnUrl)
        {
            var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
            var query = unprotectedUrl.GetQueries().ToJsonObject();
            if (query.ContainsKey("returnUrl"))
                return ExtractQuery(query["returnUrl"].GetValue<string>());

            return query;
        }
    }
}
