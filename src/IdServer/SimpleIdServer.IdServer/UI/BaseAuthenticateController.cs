// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public class BaseAuthenticateController : Controller
    {
        private readonly IdServerHostOptions _options;
        private readonly IDataProtector _dataProtector;
        private readonly IClientRepository _clientRepository;
        private readonly IAmrHelper _amrHelper;
        private readonly IUserRepository _userRepository;

        public BaseAuthenticateController(
            IOptions<IdServerHostOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            IClientRepository clientRepository,
            IAmrHelper amrHelper,
            IUserRepository userRepository)
        {
            _options = options.Value;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _clientRepository = clientRepository;
            _amrHelper = amrHelper;
            _userRepository = userRepository;
        }

        protected IClientRepository ClientRepository => _clientRepository;
        protected IUserRepository UserRepository => _userRepository;
        protected IdServerHostOptions Options => _options;

        protected string Unprotect(string returnUrl)
        {
            var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
            return unprotectedUrl;
        }

        protected void SetSuccessMessage(string msg)
        {
            ViewBag.SuccessMessage = msg;
        }

        protected async Task<IActionResult> Authenticate(string returnUrl, string currentAmr, User user, CancellationToken token, bool rememberLogin = false)
        {
            var unprotectedUrl = Unprotect(returnUrl);
            var query = unprotectedUrl.GetQueries().ToJsonObject();
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var client = await _clientRepository.Query().FirstOrDefaultAsync(c => c.ClientId == clientId, token);
            var acr = await _amrHelper.FetchDefaultAcr(acrValues, requestedClaims, client, token);
            string amr;
            if (acr == null || string.IsNullOrWhiteSpace(amr = _amrHelper.FetchNextAmr(acr, currentAmr)))
            {
                return await Sign(unprotectedUrl, currentAmr, user, token, rememberLogin);
            }

            return RedirectToAction("Index", "Authenticate", new { area = amr, ReturnUrl = returnUrl });
        }

        protected async Task<IActionResult> Sign(string returnUrl, string currentAmr, User user, CancellationToken token, bool rememberLogin = false)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(_options.CookieAuthExpirationTimeInSeconds);
            var offset = DateTimeOffset.UtcNow.AddSeconds(_options.CookieAuthExpirationTimeInSeconds);
            var claims = user.Claims;
            var claimsIdentity = new ClaimsIdentity(claims, currentAmr);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            user.AddSession(expirationDateTime);
            await _userRepository.SaveChanges(token);
            Response.Cookies.Append(_options.SessionCookieName, user.ActiveSession.SessionId, new CookieOptions
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

            return Redirect(returnUrl);
        }
    }
}
