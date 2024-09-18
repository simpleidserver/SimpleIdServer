// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs
{
    public class ApplicationProvidersController : Controller
    {
        public IActionResult Index()
        {
            // Display list of configured application providers.
            return View();
        }

        public IActionResult View(string id)
        {
            // display only one application provider.
            return View();
        }
    }
}
