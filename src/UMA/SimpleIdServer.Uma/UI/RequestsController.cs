// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Uma.UI.ViewModels;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.UI
{
    [Authorize("IsConnected")]
    public class RequestsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string idToken = await HttpContext.GetTokenAsync("id_token");
            return View(new RequestsIndexViewModel(idToken));
        }

        public async Task<IActionResult> Received()
        {
            string idToken = await HttpContext.GetTokenAsync("id_token");
            return View(new RequestsReceivedViewModel(idToken));
        }
    }
}
