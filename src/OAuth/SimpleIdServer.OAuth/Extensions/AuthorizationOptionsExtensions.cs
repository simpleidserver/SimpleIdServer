// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddDefaultOAUTHAuthorizationPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
        }
    }
}
