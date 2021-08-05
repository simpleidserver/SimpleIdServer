// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Sp.Startup.Controllers
{
    public class ExternalAuthenticateController : Controller
    {
        private const string SCHEME_NAME = "scheme";
        private const string RETURN_URL_NAME = "returnUrl";

        [HttpGet]
        public IActionResult Login(string scheme, string returnUrl)
        {
            var items = new Dictionary<string, string>
            {
                { SCHEME_NAME, scheme }
            };
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                items.Add(RETURN_URL_NAME, returnUrl);
            }
            var props = new AuthenticationProperties(items)
            {
                RedirectUri = Url.Action(nameof(Callback)),
            };
            return Challenge(props, scheme);
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var currentDateTime = DateTime.UtcNow;
            var result = await HttpContext.AuthenticateAsync("ExternalAuthentication");
            await HttpContext.SignOutAsync("ExternalAuthentication");
            var identity = result.Principal.Identity as ClaimsIdentity;
            var claimsIdentity = new ClaimsIdentity(identity.Claims, "externalAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var returnUrl = "~/";
            if (result.Properties.Items.ContainsKey(RETURN_URL_NAME))
            {
                returnUrl = result.Properties.Items[RETURN_URL_NAME];
            }

            await HttpContext.SignInAsync(claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = currentDateTime.AddSeconds(60 * 5)
            });
            return Redirect(returnUrl);
        }
    }
}
