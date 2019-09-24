// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OpenID.Exceptions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.UI;
using SimpleIdServer.UI.Authenticate.Sms.Services;
using SimpleIdServer.UI.Authenticate.Sms.ViewModels;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SimpleIdServer.UI.Authenticate.Sms.Controllers
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private readonly ISmsAuthService _smsAuthService;

        public AuthenticateController(IDataProtectionProvider dataProtectionProvider, IOAuthClientQueryRepository oauthClientRepository, IAmrHelper amrHelper,
            ISmsAuthService smsAuthService) : base(dataProtectionProvider, oauthClientRepository, amrHelper)
        {
            _smsAuthService = smsAuthService;
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
                return View(new AuthenticateViewModel(returnUrl, loginHint));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AuthenticateViewModel viewModel)
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

                    await _smsAuthService.SendConfirmationCode(viewModel.PhoneNumber);
                    SetSuccessMessage("confirmationcode_sent");
                    return View(viewModel);
                default:
                    viewModel.CheckRequiredFields(ModelState);
                    viewModel.CheckConfirmationCode(ModelState);
                    if (!ModelState.IsValid)
                    {
                        return View(viewModel);
                    }

                    try
                    {
                        var user = await _smsAuthService.Authenticate(viewModel.PhoneNumber, viewModel.ConfirmationCode);
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
}