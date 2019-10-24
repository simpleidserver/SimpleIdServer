// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Uma.UI.ViewModels;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.UI
{
    [Authorize("IsAuthenticated")]
    public class ResourcesController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string idToken = await HttpContext.GetTokenAsync("id_token");
            return View(new ResourcesIndexViewModel(idToken));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            string idToken = await HttpContext.GetTokenAsync("id_token");
            return View(new ResourcesEditViewModel(idToken, id));
        }
    }
}
