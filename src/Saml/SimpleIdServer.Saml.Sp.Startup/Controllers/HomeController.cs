// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Saml.Sp.Startup.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Sp.Startup.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public HomeController(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task<IActionResult> Index()
        {
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var externalIdProviders = schemes.Where(s => !string.IsNullOrWhiteSpace(s.DisplayName));
            var viewModel = new HomeViewModel
            {
                ExternalIdsProviders = externalIdProviders.Select(e => new ExternalIdProvider
                {
                    AuthenticationScheme = e.Name,
                    DisplayName = e.DisplayName
                }).ToList()
            };
            return View(viewModel);
        }
    }
}
