// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Website.Controllers
{
    public class LogoutController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IRealmStore _realmStore;
        private readonly IdServerWebsiteOptions _options;

        public LogoutController(IDistributedCache distributedCache, IRealmStore realmStore, IOptions<IdServerWebsiteOptions> options)
        {
            _distributedCache = distributedCache;
            _realmStore = realmStore;
            _options = options.Value;
        }

        [Route("logout")]
        public IActionResult SignOut()
        {
            var redirectUri = Url.Action("OidcSignoutCallback");
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = redirectUri
            }, "oidc");
        }

        [Route("oidccallback")]
        public IActionResult OidcSignoutCallback()
        {
            var redirectUri = "~/";
            if (_options.IsReamEnabled) redirectUri = $"~/{_realmStore.Realm}/clients";
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = Url.Content(redirectUri)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpPost]
        [Route("bc-logout")]
        [AllowAnonymous]
        public async Task<IActionResult> BackChannelLogout([FromForm] BackChannelLogoutRequest request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LogoutToken)) return BadRequest();
            var logoutToken = request.LogoutToken;
            var handler = new JsonWebTokenHandler();
            var jwt = handler.ReadJsonWebToken(logoutToken);
            var sessionId = jwt.Claims.First(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sid)?.Value;
            if(!string.IsNullOrWhiteSpace(sessionId)) await _distributedCache.SetStringAsync(sessionId, "disconnected");
            return Ok();
        }
    }

    public class BackChannelLogoutRequest
    {
        [FromForm(Name = "logout_token")]
        public string LogoutToken { get; set; }
    }
}
