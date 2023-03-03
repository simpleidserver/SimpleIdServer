// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.AuthProviders;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    [Area(Constants.Areas.Password)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IPasswordAuthService _passwordAuthService;

        public AuthenticateController(IAuthenticationSchemeProvider authenticationSchemeProvider, IPasswordAuthService passwordAuthService, IOptions<IdServerHostOptions> options, IDataProtectionProvider dataProtectionProvider, IClientRepository clientRepository, IAmrHelper amrHelper, IUserRepository userRepository, IUserTransformer userTransformer) : base(options, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
            _passwordAuthService = passwordAuthService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            try
            {
                var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
                var externalIdProviders = ExternalProviderHelper.GetExternalAuthenticationSchemes(schemes);
                if (IsProtected(returnUrl))
                {
                    var query = ExtractQuery(returnUrl);
                    var clientId = query.GetClientIdFromAuthorizationRequest();
                    var client = await ClientRepository.Query().Include(c => c.Translations).FirstOrDefaultAsync(c => c.ClientId == clientId, cancellationToken);
                    var loginHint = query.GetLoginHintFromAuthorizationRequest();
                    return View(new AuthenticatePasswordViewModel(
                    loginHint,
                        returnUrl,
                        client.ClientName,
                        client.LogoUri,
                        client.TosUri,
                        client.PolicyUri,
                        externalIdProviders.Select(e => new ExternalIdProvider
                        {
                            AuthenticationScheme = e.Name,
                            DisplayName = e.DisplayName
                        }).ToList()));
                }

                return View(new AuthenticatePasswordViewModel(
                    returnUrl,
                    externalIdProviders.Select(e => new ExternalIdProvider
                    {
                        AuthenticationScheme = e.Name,
                        DisplayName = e.DisplayName
                    }).ToList()));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AuthenticatePasswordViewModel viewModel, CancellationToken token)
        {
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            viewModel.Check(ModelState);
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var user = await _passwordAuthService.Authenticate(viewModel.Login, viewModel.Password, token);
                return await Authenticate(viewModel.ReturnUrl, Constants.Areas.Password, user, token, viewModel.RememberLogin);
            }
            catch (CryptographicException)
            {
                ModelState.AddModelError("invalid_request", "invalid_request");
                return View(viewModel);
            }
            catch (BaseUIException ex)
            {
                ModelState.AddModelError(ex.Code, ex.Code);
                return View(viewModel);
            }
        }
    }
}
