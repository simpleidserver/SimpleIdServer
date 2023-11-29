// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;

namespace SimpleIdServer.IdServer.Api.Theme;

public class ThemesController : BaseController
{
    private const string orangeStyle = ".orange { " +
           "    --bs-primary-rgb: 255, 132, 52; " +
           "    --bs-primary: #ff8434ff; " +
           "    --sid-separator-bg: #eee; " +
           "} " +
           "body.orange { " +
           "    background-color: #f3f3f3; " +
           "} " +
           ".orange .navbar-nav { " +
           "    --bs-nav-link-color: #e7e2e2; " +
           "    --bs-nav-link-hover-color: white; " +
           "} " +
           ".orange .btn-primary { " +
           "    --bs-btn-bg: #ff8434ff; " +
           "    --bs-btn-active-bg: #ff8434ff; " +
           "    --bs-btn-disabled-bg: #ff8434ff; " +
           "    --bs-btn-hover-border-color: #ff8434ff; " +
           "    --bs-btn-disabled-border-color: #ff8434ff; " +
           "    --bs-btn-active-border-color: #ff8434ff; " +
           "    --bs-btn-border-color: #ff8434ff; " +
           "    --bs-btn-hover-bg: #ee5f00ff; " +
           "} " +
           ".orange .dropdown-menu { " +
           "    --bs-dropdown-link-active-bg: #ff8434ff; " +
           "}";

    [HttpGet]
    public IActionResult Get([FromRoute] string prefix, string id)
    {
        return Content(orangeStyle, "text/css");
    }
}