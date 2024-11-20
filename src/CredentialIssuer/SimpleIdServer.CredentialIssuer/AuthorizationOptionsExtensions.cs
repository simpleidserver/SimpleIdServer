// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Microsoft.AspNetCore.Authorization;

public static class AuthorizationOptionsExtensions
{
    public static AuthorizationOptions AddDefaultCredentialIssuerAuthorization(this AuthorizationOptions b, string bearer = "Bearer")
    {
        b.AddPolicy("WebsiteAuthenticated", p =>
        {
            p.RequireAuthenticatedUser();
        });
        b.AddPolicy("ApiAuthenticated", p =>
        {
            p.AuthenticationSchemes.Clear();
            p.AuthenticationSchemes.Add(bearer);
            p.RequireAuthenticatedUser();
        });
        b.AddPolicy("credconfs", p =>
        {
            p.AuthenticationSchemes.Clear();
            p.AuthenticationSchemes.Add(bearer);
            p.RequireClaim("scope", "credconfs");
        });
        b.AddPolicy("credinstances", p =>
        {
            p.AuthenticationSchemes.Clear();
            p.AuthenticationSchemes.Add(bearer);
            p.RequireClaim("scope", "credinstances");
        });
        b.AddPolicy("deferredcreds", p =>
        {
            p.AuthenticationSchemes.Clear();
            p.AuthenticationSchemes.Add(bearer);
            p.RequireClaim("scope", "deferredcreds");
        });

        return b;
    }
}