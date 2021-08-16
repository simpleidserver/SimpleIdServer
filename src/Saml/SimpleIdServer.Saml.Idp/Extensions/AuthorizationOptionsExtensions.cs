// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using SimpleIdServer.Saml.Idp;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddDefaultSamlIdpAuthorizationPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy("ManageRelyingParties", p =>
            {
                p.AddAuthenticationSchemes(SamlIdpConstants.AuthenticationScheme);
                p.RequireClaim(SimpleIdServer.Jwt.Constants.OAuthClaims.Scopes, "manage_relying_parties");
            });
        }
    }
}
