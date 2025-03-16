// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Website
{
    public static class WebsiteConfiguration
    {
        public static List<string> StandardUsers = new List<string>
        {
            "administrator"
        };

        public static List<string> StandardClients = new List<string>
        {
            "website",
            "urn:website"
        };

        public static List<string> StandardScopes = new List<string>
        {
            Config.DefaultScopes.OpenIdScope.Name,
            Config.DefaultScopes.Profile.Name,
            Config.DefaultScopes.SAMLProfile.Name
        };
    }
}
