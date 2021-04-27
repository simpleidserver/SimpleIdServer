// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Helpers;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.UI
{
    public class BaseAuthenticateController : Controller
    {
        private readonly OpenIDHostOptions _options;
        private readonly IDataProtector _dataProtector;
        private readonly IOAuthClientQueryRepository _oauthClientRepository;
        private readonly IAmrHelper _amrHelper;
        private readonly IOAuthUserCommandRepository _oauthUserCommandRepository;

        public BaseAuthenticateController(
            IOptions<OpenIDHostOptions> options,
            IDataProtectionProvider dataProtectionProvider, 
            IOAuthClientQueryRepository oauthClientRepository,
            IAmrHelper amrHelper,
            IOAuthUserCommandRepository oauthUserCommandRepository)
        {
            _options = options.Value;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _oauthClientRepository = oauthClientRepository;
            _amrHelper = amrHelper;
            _oauthUserCommandRepository = oauthUserCommandRepository;
        }

        protected IOAuthClientQueryRepository OAuthClientQueryRepository => _oauthClientRepository;

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
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var client = (OpenIdClient)await _oauthClientRepository.FindOAuthClientById(clientId, token);
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, requestedClaims, client, token);
            string amr;
            if (acr == null || string.IsNullOrWhiteSpace(amr = _amrHelper.FetchNextAmr(acr, currentAmr)))
            {
                var currentDateTime = DateTime.UtcNow;
                var expirationDateTime = currentDateTime.AddSeconds(_options.CookieAuthExpirationTimeInSeconds);
                var claims = user.ToClaims();
                var claimsIdentity = new ClaimsIdentity(claims, currentAmr);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                user.TryAddSession(expirationDateTime);
                await _oauthUserCommandRepository.Update(user, token);
                await _oauthUserCommandRepository.SaveChanges(token);
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = rememberLogin,
                    ExpiresUtc = expirationDateTime
                });

                return Redirect(unprotectedUrl);
            }

            return RedirectToAction("Index", "Authenticate", new { area = amr, ReturnUrl = returnUrl });
        }
    }
}
