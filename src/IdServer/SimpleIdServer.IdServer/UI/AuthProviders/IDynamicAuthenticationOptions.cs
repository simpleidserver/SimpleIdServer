// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;

namespace SimpleIdServer.IdServer.UI.AuthProviders
{
    public interface IDynamicAuthenticationOptions<TOptions> where TOptions : AuthenticationSchemeOptions, new()
    {
        TOptions Convert();
    }
}
