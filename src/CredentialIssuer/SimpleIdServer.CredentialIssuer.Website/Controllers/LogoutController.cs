// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.CredentialIssuer.Website.Controllers;

public class LogoutController : Controller
{
    [Route("logout")]
    public IActionResult SignOut()
    {
        var redirectUri = Url.Action("OidcSignoutCallback");
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = redirectUri
        }, "oidc");
    }


    [Route("oidccallback")]
    public IActionResult OidcSignoutCallback()
    {
        var redirectUri = "~/";
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = Url.Content(redirectUri)
        }, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
