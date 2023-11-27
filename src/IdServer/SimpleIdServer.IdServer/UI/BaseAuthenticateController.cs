// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Events;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.UI.Services;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public class BaseAuthenticateController : BaseController
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionResitory _userSessionRepository;
        private readonly IAmrHelper _amrHelper;
        private readonly IBusControl _busControl;
        private readonly IUserTransformer _userTransformer;
        private readonly IDataProtector _dataProtector;
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IdServerHostOptions _options;

        public BaseAuthenticateController(
            IClientRepository clientRepository,
            IUserRepository userRepository,
            IUserSessionResitory userSessionResitory,
            IAmrHelper amrHelper,
            IBusControl busControl,
            IUserTransformer userTransformer,
            IDataProtectionProvider dataProtectionProvider,
            IAuthenticationHelper authenticationHelper,
            IOptions<IdServerHostOptions> options)
        {
            _clientRepository = clientRepository;
            _userRepository = userRepository;
            _userSessionRepository = userSessionResitory;
            _amrHelper = amrHelper;
            _busControl = busControl;
            _userTransformer = userTransformer;
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
            _authenticationHelper = authenticationHelper;
            _options = options.Value;
        }

        protected IUserRepository UserRepository => _userRepository;
        protected IClientRepository ClientRepository => _clientRepository;
        protected IdServerHostOptions Options => _options;
        protected IAmrHelper AmrHelper => _amrHelper;
        protected IBusControl Bus => _busControl;
        protected IAuthenticationHelper AuthenticationHelper => _authenticationHelper;

        protected JsonObject ExtractQuery(string returnUrl) => ExtractQueryFromUnprotectedUrl(Unprotect(returnUrl));

        protected JsonObject ExtractQueryFromUnprotectedUrl(string returnUrl)
        {
            var query = returnUrl.GetQueries().ToJsonObject();
            if (query.ContainsKey("returnUrl"))
                return ExtractQuery(query["returnUrl"].GetValue<string>());

            return query;
        }

        protected string Unprotect(string returnUrl)
        {
            var unprotectedUrl = _dataProtector.Unprotect(returnUrl);
            return unprotectedUrl;
        }

        protected bool IsProtected(string returnUrl)
        {
            try
            {
                _dataProtector.Unprotect(returnUrl);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected async Task<IActionResult> Authenticate(string realm, string returnUrl, string currentAmr, User user, CancellationToken token, bool rememberLogin = false)
        {
            if (!IsProtected(returnUrl))
            {
                return await Sign(realm, returnUrl, currentAmr, user, null, token, rememberLogin);
            }

            var unprotectedUrl = Unprotect(returnUrl);
            var query = ExtractQuery(returnUrl);
            var acrValues = query.GetAcrValuesFromAuthorizationRequest();
            var clientId = query.GetClientIdFromAuthorizationRequest();
            var requestedClaims = query.GetClaimsFromAuthorizationRequest();
            var client = await _clientRepository.Query().Include(c => c.Realms).FirstOrDefaultAsync(c => c.ClientId == clientId && c.Realms.Any(r => r.Name == realm), token);
            var acr = await _amrHelper.FetchDefaultAcr(realm, acrValues, requestedClaims, client, token);
            string amr;
            if (acr == null || string.IsNullOrWhiteSpace(amr = _amrHelper.FetchNextAmr(acr, currentAmr)))
            {
                return await Sign(realm, unprotectedUrl, currentAmr, user, clientId, token, rememberLogin);
            }

            var allAmr = acr.AuthenticationMethodReferences;
            var login = _authenticationHelper.GetLogin(user);
            HttpContext.Response.Cookies.Append(Constants.DefaultCurrentAmrCookieName, JsonSerializer.Serialize(new AmrAuthInfo(user.Id, login, user.Email, user.OAuthUserClaims.Select(c => new KeyValuePair<string, string>(c.Name, c.Value)).ToList(), allAmr, amr)));
	        await _userRepository.SaveChanges(token);
            return RedirectToAction("Index", "Authenticate", new { area = amr, ReturnUrl = returnUrl });
        }

        protected async Task<IActionResult> Sign(string realm, string returnUrl, string currentAmr, User user, string clientId, CancellationToken token, bool rememberLogin = false)
        {
            await AddSession(realm, user, clientId, token);
            var offset = DateTimeOffset.UtcNow.AddSeconds(_options.CookieAuthExpirationTimeInSeconds);
            var claims = _userTransformer.Transform(user);
            var claimsIdentity = new ClaimsIdentity(claims, currentAmr);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
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

            await _busControl.Publish(new UserLoginSuccessEvent
            {
                Realm = realm,
                UserName = user.Name,
                Amr = currentAmr
            }, token);
            return Redirect(returnUrl);
        }

        protected async Task AddSession(string realm, User user, string clientId, CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            var expirationDateTime = currentDateTime.AddSeconds(_options.CookieAuthExpirationTimeInSeconds);
            var session = new UserSession
            {
                SessionId = Guid.NewGuid().ToString(),
                AuthenticationDateTime = DateTime.UtcNow,
                ExpirationDateTime = expirationDateTime,
                State = UserSessionStates.Active,
                Realm = realm,
                UserId = user.Id,
                ClientIds = new List<string> { }
            };
            _userSessionRepository.Add(session);
            await _userSessionRepository.SaveChanges(cancellationToken);
            await _userRepository.SaveChanges(cancellationToken);
            Response.Cookies.Append(_options.GetSessionCookieName(), session.SessionId, new CookieOptions
            {
                Secure = true,
                HttpOnly = false,
                SameSite = SameSiteMode.None
            });
        }
    }
}
