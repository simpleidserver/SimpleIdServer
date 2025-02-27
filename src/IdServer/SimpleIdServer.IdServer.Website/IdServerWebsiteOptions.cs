// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;

namespace SimpleIdServer.IdServer.Website
{
    public class IdServerWebsiteOptions
    {
        public string IdServerBaseUrl { get; set; } = "https://localhost:5001";
        public string SCIMUrl { get; set; } = "https://localhost:5003";
        public bool IsReamEnabled { get; set; } = true;
    }
}
