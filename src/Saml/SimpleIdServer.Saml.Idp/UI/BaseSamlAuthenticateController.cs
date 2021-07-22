// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Common.Extensions;
using SimpleIdServer.Saml.Idp.Persistence;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.UI
{
    public class BaseSamlAuthenticateController : Controller
    {
        private readonly SamlIdpOptions _samlIdpOptions;
        private readonly IUserRepository _userRepository;

        public BaseSamlAuthenticateController(
            IOptions<SamlIdpOptions> samlIdpIOptions,
            IUserRepository userRepository)
        {
            _samlIdpOptions = samlIdpIOptions.Value;
            _userRepository = userRepository;
        }

        protected async Task Authenticate(string currentAmr, User user, CancellationToken cancellationToken, bool rememberLogin = false)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(_samlIdpOptions.CookieAuthExpirationTimeInSeconds);
            var offset = DateTimeOffset.UtcNow.AddSeconds(_samlIdpOptions.CookieAuthExpirationTimeInSeconds);
            var claims = user.ToClaims();
            var claimsIdentity = new ClaimsIdentity(claims, currentAmr);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            user.AddSession(expirationDateTime);
            await _userRepository.Update(user, cancellationToken);
            await _userRepository.SaveChanges(cancellationToken);
            Response.Cookies.Append(_samlIdpOptions.SessionCookieName, user.GetActiveSession().SessionId, new CookieOptions
            {
                Secure = true,
                HttpOnly = false,
                SameSite = SameSiteMode.None
            });
            if (rememberLogin)
            {
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true
                });
            }
            else
            {
                await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = offset
                });
            }
        }
    }
}
