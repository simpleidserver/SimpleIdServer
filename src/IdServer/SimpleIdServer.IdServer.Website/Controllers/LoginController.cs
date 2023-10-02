// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.Website.Controllers
{
    public class LoginController : Controller
    {
        [Route("login")]
        public IActionResult Login(string acrValues)
        {
            var items = new Dictionary<string, string>
            {
                { "scheme", "oidc" },
            };
            var props = new AuthenticationProperties(items);
            props.Parameters.Add("prompt", "login");
            props.Parameters.Add("acr_values", acrValues);
            return Challenge(props, "oidc");
        }
    }
}
