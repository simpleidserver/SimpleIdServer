// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Common.Exceptions;
using SimpleIdServer.Saml.DTOs;
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Idp;
using SimpleIdServer.Saml.Idp.Persistence;
using SimpleIdServer.Saml.Idp.UI;
using SimpleIdServer.Saml.UI.Authenticate.LoginPassword.Services;
using SimpleIdServer.Saml.UI.Authenticate.LoginPassword.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleIdServer.Saml.UI.Authenticate.LoginPassword.Controllers
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseSamlAuthenticateController
    {
        private readonly IPasswordSamlAuthService _passwordSamlAuthService;

        public AuthenticateController(
            IOptions<SamlIdpOptions> options,
            IUserRepository userRepository,
            IPasswordSamlAuthService passwordSamlAuthService) : base(options, userRepository)
        {
            _passwordSamlAuthService = passwordSamlAuthService;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] SAMLRequestDto singleSignOnParameter)
        {
            var viewModel = new LoginViewModel { Parameter = singleSignOnParameter };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel viewModel, CancellationToken cancellationToken)
        {
            viewModel.Check(ModelState);
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var user = await _passwordSamlAuthService.Authenticate(viewModel.Login, viewModel.Password, cancellationToken);
                await Authenticate(Constants.AMR, user, cancellationToken, viewModel.RememberLogin);
                var url = $"{Request.GetAbsoluteUriWithVirtualPath()}/{SimpleIdServer.Saml.Constants.RouteNames.SingleSignOn}/Login?SAMLRequest={HttpUtility.UrlEncode(viewModel.Parameter.SAMLRequest)}&RelayState={HttpUtility.UrlEncode(viewModel.Parameter.RelayState)}";
                return Redirect(url);
            }
            catch(BaseUIException ex)
            {
                ModelState.AddModelError(ex.Code, ex.Code);
                return View(viewModel);
            }
        }
    }
}
