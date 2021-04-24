// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.UI;
using SimpleIdServer.UI.Authenticate.LoginPassword.Services;
using SimpleIdServer.UI.Authenticate.LoginPassword.ViewModels;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.LoginPassword.Controllers
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private readonly IPasswordAuthService _passwordAuthService;
        private readonly ITranslationHelper _translationHelper;

        public AuthenticateController(
            IPasswordAuthService passwordAuthService,
            ITranslationHelper translationHelper,
            IDataProtectionProvider dataProtectionProvider, 
            IAmrHelper amrHelper, 
            IOAuthClientQueryRepository oauthClientRepository,
            IOAuthUserCommandRepository oauthUserCommandRepository) : base(dataProtectionProvider, oauthClientRepository, oauthUserCommandRepository, amrHelper)
        {
            _passwordAuthService = passwordAuthService;
            _translationHelper = translationHelper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }

            try
            {
                var query = Unprotect(returnUrl).GetQueries().ToJObj();
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var client = await OAuthClientQueryRepository.FindOAuthClientById(clientId, cancellationToken);
                var loginHint = query.GetLoginHintFromAuthorizationRequest();
                return View(new AuthenticateViewModel(
                    loginHint, 
                    returnUrl,
                    _translationHelper.Translate(client.ClientNames),
                    _translationHelper.Translate(client.LogoUris),
                    _translationHelper.Translate(client.TosUris),
                    _translationHelper.Translate(client.PolicyUris)));
            }
            catch(CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AuthenticateViewModel viewModel, CancellationToken token)
        {
            if (viewModel == null)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }

            viewModel.Check(ModelState);
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var user = await _passwordAuthService.Authenticate(viewModel.Login, viewModel.Password, token);
                return await Authenticate(viewModel.ReturnUrl, Constants.AMR, user, token, viewModel.RememberLogin);
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