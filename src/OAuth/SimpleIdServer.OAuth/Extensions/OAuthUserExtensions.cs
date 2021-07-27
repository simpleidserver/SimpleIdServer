// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Extensions;
using SimpleIdServer.OAuth.Domains;
using System.Linq;
using System.Security.Claims;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class OAuthUserExtensions
    {
        public static OAuthUser BuildOAuthUser(this ClaimsPrincipal claimsPrincipal, string scheme)
        {
            var userClaims = claimsPrincipal.BuildClaims();
            var claimSub = userClaims.FirstOrDefault(c => c.Type == SimpleIdServer.Jwt.Constants.UserClaims.Subject);
            var claimName = userClaims.FirstOrDefault(c => c.Type == SimpleIdServer.Jwt.Constants.UserClaims.Name);
            string sub = claimSub == null ? null : claimSub.Value;
            string name = claimName == null ? null : claimName.Value;
            var user = OAuthUser.Create(name, null);
            user.UpdateClaims(userClaims);
            user.AddExternalAuthProvider(scheme, sub);
            return user;
        }
    }
}
