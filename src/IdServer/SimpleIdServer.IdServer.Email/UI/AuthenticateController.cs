// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Email.UI.Services;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI;
using SimpleIdServer.IdServer.UI.Services;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Email.UI
{
    [Area(Constants.AMR)]
    public class AuthenticateController : BaseAuthenticateController
    {
        private readonly IEmailAuthService _emailAuthService;

        public AuthenticateController(
            IEmailAuthService emailAuthService, 
            IOptions<IdServerHostOptions> options, 
            IDataProtectionProvider dataProtectionProvider, 
            IClientRepository clientRepository, 
            IAmrHelper amrHelper,
            IUserRepository userRepository, 
            IUserTransformer userTransformer, 
            IBusControl busControl) : base(options, dataProtectionProvider, clientRepository, amrHelper, userRepository, userTransformer, busControl)
        {
            _emailAuthService = emailAuthService;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] string prefix, string returnUrl, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(returnUrl)) return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            try
            {
                prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
                var query = ExtractQuery(returnUrl);
                var clientId = query.GetClientIdFromAuthorizationRequest();
                var client = await ClientRepository.Query().Include(c => c.Realms).Include(c => c.Translations).FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == prefix), cancellationToken);
                var amrInfo = await ResolveAmrInfo(query, prefix, client, cancellationToken);
                var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, cancellationToken);
                var loginHint = query.GetLoginHintFromAuthorizationRequest();
                bool isEmailMissing = false;
                if (authenticatedUser != null && string.IsNullOrWhiteSpace(authenticatedUser.Email)) isEmailMissing = true;
                else if (authenticatedUser != null) loginHint = authenticatedUser.Email;
                return View(new AuthenticateEmailViewModel(returnUrl,
                    prefix,
                    loginHint,
                    client.ClientName,
                    client.LogoUri,
                    client.TosUri,
                    client.PolicyUri,
                    isEmailMissing,
                    authenticatedUser != null,
                    amrInfo));
            }
            catch (CryptographicException)
            {
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromRoute] string prefix, AuthenticateEmailViewModel viewModel, CancellationToken token)
        {
            viewModel.Realm = prefix;
            prefix = prefix ?? SimpleIdServer.IdServer.Constants.DefaultRealm;
            if (viewModel == null)
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });

            var amrInfo = GetAmrInfo();
            var authenticatedUser = await FetchAuthenticatedUser(prefix, amrInfo, token);
            viewModel.CheckRequiredFields(ModelState);
            viewModel.CheckEmail(ModelState, authenticatedUser);
            switch (viewModel.Action)
            {
                case "SENDCONFIRMATIONCODE":
                    if (!ModelState.IsValid)
                        return View(viewModel);

                    try
                    {
                        await _emailAuthService.SendCode(viewModel.Email, token);
                        SetSuccessMessage("confirmationcode_sent");
                        return View(viewModel);
                    }
                    catch (BaseUIException ex)
                    {
                        ModelState.AddModelError(ex.Code, ex.Code);
                        return View(viewModel);
                    }
                default:
                    viewModel.CheckConfirmationCode(ModelState);
                    if (!ModelState.IsValid)
                        return View(viewModel);

                    try
                    {
                        var user = await _emailAuthService.Authenticate(viewModel.Email, viewModel.OTPCode.Value, token);
                        return await Authenticate(prefix, viewModel.ReturnUrl, Constants.AMR, user, token, viewModel.RememberLogin);
                    }
                    catch (CryptographicException)
                    {
                        ModelState.AddModelError("invalid_request", "cryptographic_error");
                        return View(viewModel);
                    }
                    catch (BaseUIException ex)
                    {
                        ModelState.AddModelError(ex.Code, ex.Code);
                        await Bus.Publish(new UserLoginFailureEvent
                        {
                            Realm = prefix,
                            Amr = Constants.AMR,
                            Login = viewModel.Email
                        });
                        return View(viewModel);
                    }
            }
        }
    }
}
