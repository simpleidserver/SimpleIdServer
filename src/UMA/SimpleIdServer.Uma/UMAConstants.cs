// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;

namespace SimpleIdServer.Uma
{
    public static class UMAConstants
    {
        public const string SignInScheme = "SimpleIdServerUMA";
        public const string OAuthSignInScheme = "SimpleIdServerUmaOauth";
        public const string ChallengeAuthenticationScheme = "SimpleIdServerUMA.Challenge";

        public static class EndPoints
        {
            public const string ResourcesAPI = "rreguri";
            public const string PermissionsAPI = "perm";
            public const string RequestsAPI = "reqs";
            public const string ManagementAPI = "management";
        }

        public static class StandardUMAScopes
        {
            public static OAuthScope UmaProtection = new OAuthScope
            {
                CreateDateTime = DateTime.UtcNow,
                IsExposedInConfigurationEdp = true,
                Name = "uma_protection",
                UpdateDateTime = DateTime.UtcNow
            };
        }
    }
}
