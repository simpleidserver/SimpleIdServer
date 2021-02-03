// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Options;
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

        public AuthenticateController(IPasswordAuthService passwordAuthService, IDataProtectionProvider dataProtectionProvider, IAmrHelper amrHelper, IOAuthClientQueryRepository oauthClientRepository) : base(dataProtectionProvider, oauthClientRepository, amrHelper)
        {
            _passwordAuthService = passwordAuthService;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }

            try
            {
                var query = Unprotect(returnUrl).GetQueries().ToJObj();
                var loginHint = query.GetLoginHintFromAuthorizationRequest();
                return View(new AuthenticateViewModel(loginHint, returnUrl));
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
                return await Authenticate(viewModel.ReturnUrl, Constants.AMR, user, viewModel.RememberLogin);
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