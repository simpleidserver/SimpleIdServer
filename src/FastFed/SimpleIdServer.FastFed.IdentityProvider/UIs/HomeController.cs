// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.FastFed.IdentityProvider.UIs;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
