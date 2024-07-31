// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SimpleIdServer.IdServer.Website.Controllers
{
    public class LogoutController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public LogoutController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;

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
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = Url.Content("~/")
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
