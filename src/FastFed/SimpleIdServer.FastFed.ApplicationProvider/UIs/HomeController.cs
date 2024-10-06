// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.ApplicationProvider.UIs.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs;

public class HomeController : Controller
{
    public const string SCHEME_NAME = "scheme";
    public const string RETURN_URL_NAME = "returnUrl";
    private readonly FastFedApplicationProviderOptions _options;
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public HomeController(
        IOptions<FastFedApplicationProviderOptions> options,
        IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _options = options.Value;
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Authenticate()
    {
        var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
        var externalSchemes = schemes.Where(s => !string.IsNullOrWhiteSpace(s.DisplayName) && s.Name != _options.AuthScheme.Openid);
        var viewModel = new AuthenticateViewModel
        {
            ExternalIdProviders = externalSchemes.Select(s => new ExternalIdProvider
            {
                AuthenticationScheme = s.Name,
                DisplayName = s.DisplayName
            }).ToList()
        };
        return View(viewModel);
    }

    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/"
        }, _options.AuthScheme.Openid);
    }

    public IActionResult ExternalLogin(string scheme, CancellationToken cancellationToken)
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/"
        }, scheme);
    }

    public IActionResult Disconnect()
    {
        return SignOut(new[] { _options.AuthScheme.Openid, _options.AuthScheme.Cookie });
    }
}
