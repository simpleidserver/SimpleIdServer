// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI
{
    public class BaseAuthenticateController : Controller
    {
        private readonly IDataProtector _dataProtector;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IOAuthUserCommandRepository _oauthUserCommandRepository;
        private readonly IAmrHelper _amrHelper;

        public BaseAuthenticateController(
            IDataProtectionProvider dataProtectionProvider, 
            IOAuthClientQueryRepository oauthClientRepository,
            IOAuthUserCommandRepository oAuthUserCommandRepository,
            IAmrHelper amrHelper)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _oauthClientRepository = oauthClientRepository;
            _oauthUserCommandRepository = oAuthUserCommandRepository;
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

        protected async Task<IActionResult> Authenticate(string returnUrl, string currentAmr, OAuthUser user, CancellationToken token, bool rememberLogin = false)
        {
            var unprotectedUrl = Unprotect(returnUrl);
            var query = unprotectedUrl.GetQueries().ToJObj();
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var client = (OpenIdClient)await _oauthClientRepository.FindOAuthClientById(clientId, token);
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, client);
            string amr;
            if (acr == null || string.IsNullOrWhiteSpace(amr = _amrHelper.FetchNextAmr(acr, currentAmr)))
            {
                var claims = user.ToClaims();
                var claimsIdentity = new ClaimsIdentity(claims, currentAmr);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                user.AuthenticationTime = DateTime.UtcNow;
                await _oauthUserCommandRepository.Update(user, token);
                await _oauthUserCommandRepository.SaveChanges(token);
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = rememberLogin
                });
                return Redirect(unprotectedUrl);
            }

            return RedirectToAction("Index", "Authenticate", new { area = amr, ReturnUrl = returnUrl });
        }
    }
}
