// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Domains.Clients;
using SimpleIdServer.OAuth.Domains.Users;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI
{
    public class BaseAuthenticateController : Controller
    {
        private readonly IDataProtector _dataProtector;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IAmrHelper _amrHelper;

        public BaseAuthenticateController(IDataProtectionProvider dataProtectionProvider, IOAuthClientQueryRepository oauthClientRepository, IAmrHelper amrHelper)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _oauthClientRepository = oauthClientRepository;
            _amrHelper = amrHelper;
        }


        protected string Unprotect(string returnUrl)
        {
            var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
            return unprotectedUrl;
        }

        protected void SetSuccessMessage(string msg)
        {
            ViewBag.SuccessMessage = msg;
        }


        protected async Task<IActionResult> Authenticate(string returnUrl, string currentAmr, OAuthUser user, bool rememberLogin = false)
        {
            var unprotectedUrl = Unprotect(returnUrl);
            var query = unprotectedUrl.GetQueries().ToJObj();
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var client = await _oauthClientRepository.FindOAuthClientById(clientId);
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, client);
            string amr;
            if (acr == null || string.IsNullOrWhiteSpace(amr = _amrHelper.FetchNextAmr(acr, currentAmr)))
            {
                await HttpContext.SignInAsync(user.ToClaimsPrincipal(), new AuthenticationProperties
                {
                    IsPersistent = rememberLogin
                });
                await HttpContext.SignInAsync("MultiAccount", user.ToClaimsPrincipal());
                return Redirect(unprotectedUrl);
            }

            return RedirectToAction("Index", "Authenticate", new { area = amr, ReturnUrl = returnUrl });
        }
    }
}
