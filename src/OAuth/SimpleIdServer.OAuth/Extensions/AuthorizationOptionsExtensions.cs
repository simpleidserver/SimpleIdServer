// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using SimpleIdServer.OAuth;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddDefaultOAUTHAuthorizationPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy("IsConnected", p => p.RequireAuthenticatedUser());
            options.AddPolicy("ManageClients", p =>
            {
                p.AddAuthenticationSchemes(Constants.AuthenticationScheme);
                p.RequireClaim(SimpleIdServer.Jwt.Constants.OAuthClaims.Scopes, "manage_clients");
            });
            options.AddPolicy("ManageScopes", p =>
            {
                p.AddAuthenticationSchemes(Constants.AuthenticationScheme);
                p.RequireClaim(SimpleIdServer.Jwt.Constants.OAuthClaims.Scopes, "manage_scopes");
            });
        }
    }
}