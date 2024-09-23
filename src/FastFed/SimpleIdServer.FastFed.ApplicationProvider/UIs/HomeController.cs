// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.FastFed.ApplicationProvider.UIs;

public class HomeController : Controller
{
    private readonly FastFedApplicationProviderOptions _options;

    public HomeController(IOptions<FastFedApplicationProviderOptions> options)
    {
        _options = options.Value;
    }

    public IActionResult Index() => View();

    public IActionResult Disconnect()
    {
        return SignOut(new[] { _options.AuthScheme.Openid, _options.AuthScheme.Cookie });
    }

    public IActionResult Authenticate()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/"
        }, _options.AuthScheme.Openid);
    }
}
