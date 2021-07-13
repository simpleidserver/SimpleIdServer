// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.UI;
using SimpleIdServer.UI.Authenticate.Sms.Services;
using SimpleIdServer.UI.Authenticate.Sms.ViewModels;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.Sms.Controllers
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private readonly ISmsAuthService _smsAuthService;
        private readonly ITranslationHelper _translationHelper;

        public AuthenticateController(
            IOptions<OpenIDHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            ITranslationHelper translateHelper,
            IOAuthClientRepository oauthClientRepository, 
            IAmrHelper amrHelper, 
            ISmsAuthService smsAuthService,
            IOAuthUserRepository oauthUserCommandRepository) : base(options, dataProtectionProvider, oauthClientRepository, amrHelper, oauthUserCommandRepository)
        {
            _smsAuthService = smsAuthService;
            _translationHelper = translateHelper;
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
                return View(new AuthenticateViewModel(returnUrl,
                    loginHint,
                    _translationHelper.Translate(client.ClientNames),
                    _translationHelper.Translate(client.LogoUris),
                    _translationHelper.Translate(client.TosUris),
                    _translationHelper.Translate(client.PolicyUris)));
            }
            catch (CryptographicException)
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

            switch (viewModel.Action)
            {
                case "SENDCONFIRMATIONCODE":
                    viewModel.CheckRequiredFields(ModelState);
                    if (!ModelState.IsValid)
                    {
                        return View(viewModel);
                    }
                    
                    try
                    {
                        await _smsAuthService.SendCode(viewModel.PhoneNumber, token);
                        SetSuccessMessage("confirmationcode_sent");
                        return View(viewModel);
                    }
                    catch(BaseUIException ex)
                    {
                        ModelState.AddModelError(ex.Code, ex.Code);
                        return View(viewModel);
                    }
                default:
                    viewModel.CheckRequiredFields(ModelState);
                    viewModel.CheckConfirmationCode(ModelState);
                    if (!ModelState.IsValid)
                    {
                        return View(viewModel);
                    }

                    try
                    {
                        var user = await _smsAuthService.Authenticate(viewModel.PhoneNumber, viewModel.OTPCode.Value, token);
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
}