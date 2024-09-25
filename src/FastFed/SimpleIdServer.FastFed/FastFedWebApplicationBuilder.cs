// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;

namespace SimpleIdServer.FastFed;

public class FastFedWebApplicationBuilder
{
    internal FastFedWebApplicationBuilder(WebApplication webApplication)
    {
        WebApplication = webApplication;
    }

    public WebApplication WebApplication { get; private set; }
}
