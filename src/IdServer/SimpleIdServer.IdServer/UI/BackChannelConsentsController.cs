// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.BCCallback;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
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
        private readonly Helpers.IHttpClientFactory _httpClientFactory;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly ILogger<BackChannelConsentsController> _logger;

        public BackChannelConsentsController(
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            Helpers.IHttpClientFactory httpClientFactory,
            IAuthenticationHelper authenticationHelper,
            IBCAuthorizeRepository bcAuthorizeRepository,
            ILogger<BackChannelConsentsController> logger)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _clientRepository = clientRepository;
            _httpClientFactory = httpClientFactory;
            _authenticationHelper = authenticationHelper;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
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

                var viewModel = await BuildViewModel(prefix, queries, returnUrl, cancellationToken);

                return View(viewModel);
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "cryptography_error", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, ConfirmBCConsentsViewModel confirmConsentsViewModel, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
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
                var issuer = $"{Request.GetAbsoluteUriWithVirtualPath()}/{Config.DefaultEndpoints.BCCallback}";
                if(!string.IsNullOrWhiteSpace(prefix))                
                    issuer = $"{Request.GetAbsoluteUriWithVirtualPath()}/{prefix}/{Config.DefaultEndpoints.BCCallback}";

                var queries = ExtractQuery(confirmConsentsViewModel.ReturnUrl);
                viewModel = await BuildViewModel(prefix, queries, viewModel.ReturnUrl, cancellationToken);
                var parameter = new BCCallbackParameter
                {
                    ActionEnum = confirmConsentsViewModel.IsRejected ? BCCallbackActions.REJECT : BCCallbackActions.CONFIRM,
                    AuthReqId = viewModel.AuthReqId
                };
                var bcAuthorize = await _bcAuthorizeRepository.GetById(parameter.AuthReqId, cancellationToken);
                if(bcAuthorize == null)
                {
                    ModelState.AddModelError("invalid_request", "unknown_bc_authorize");
                    return View(viewModel);
                }

                var sub = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var user = await _authenticationHelper.GetUserByLogin(sub, prefix, cancellationToken);
                if(bcAuthorize.UserId != user.Id)
                {
                    ModelState.AddModelError("invalid_request", "unauthorized_bc_auth");
                    return View(viewModel);
                }

                using (var httpClient = _httpClientFactory.GetHttpClient())
                {
                    var json = JsonSerializer.Serialize(parameter);
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(issuer),
                        Content = new StringContent(JsonSerializer.Serialize(parameter), System.Text.Encoding.UTF8, "application/json")
                    };
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

                viewModel.IsConfirmed = true;
                viewModel.ConfirmationStatus = confirmConsentsViewModel.IsRejected ? ConfirmationStatus.REJECTED : ConfirmationStatus.CONFIRMED;
                return View(viewModel);
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("invalid_request", "cryptography_error");
                return View(viewModel);
            }
        }

        private async Task<BCConsentsIndexViewModel> BuildViewModel(string realm, JsonObject queries, string returnUrl, CancellationToken cancellationToken)
        {
            var viewModel = new BCConsentsIndexViewModel
            {
                AuthReqId = queries.GetAuthReqId(),
                ClientId = queries.GetClientId(),
                BindingMessage = queries.GetBindingMessage(),
                AuthorizationDetails = queries.GetAuthorizationDetailsFromAuthorizationRequest(),
                Scopes = queries.GetScopes(),
                ReturnUrl = returnUrl
            };
            var str = realm ?? Constants.DefaultRealm;
            var client = await _clientRepository.GetByClientId(str, viewModel.ClientId, cancellationToken);
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
