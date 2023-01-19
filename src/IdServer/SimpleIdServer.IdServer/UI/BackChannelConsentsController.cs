// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.UI
{
    public class BackChannelConsentsController : Controller
    {
        private readonly IDataProtector _dataProtector;

        public BackChannelConsentsController(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("Authorization");
        }

        public async Task<IActionResult> Index(string returnUrl, CancellationToken cancellationToken)
        {
            // TODO : Support acr_values
            if (string.IsNullOrWhiteSpace(returnUrl))
                return RedirectToAction("Index", "Errors", new { code = "invalid_request", ReturnUrl = $"{Request.Path}{Request.QueryString}", area = string.Empty });


            if (!User.Identity.IsAuthenticated)
            {
                returnUrl = $"{Request.GetAbsoluteUriWithVirtualPath()}{Url.Action("Index", "BackChannelConsents")}?returnUrl={returnUrl}";
                return RedirectToAction("Index", "Authenticate", new { area = Constants.Areas.Password, ReturnUrl = _dataProtector.Protect(returnUrl) });
            }

            return View();
        }
    }
}
