// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.Saml.UI.Authenticate.LoginPassword.Controllers
{
    [Area(Constants.AMR)]
    public class AuthenticateController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
