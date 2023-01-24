// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Startup.Converters;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Startup
{
    public class IdServerConfiguration
    {
        public static ICollection<Scope> Scopes => new List<Scope>
        {
            Constants.StandardScopes.OpenIdScope,
            Constants.StandardScopes.Profile
        };

        public static ICollection<User> Users => new List<User>
        {
            UserBuilder.Create("administrator", "password", "Administrator").Build()
        };

        public static ICollection<Client> Clients => new List<Client>
        {
            ClientBuilder.BuildTraditionalWebsiteClient("website", "password", "http://localhost:60001/signin-oidc").AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("bankWebsite", "password", "http://localhost:60001/signin-oidc").AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).EnableCIBAGrantType().Build()
        };

        public static ICollection<AuthenticationSchemeProvider> Providers => new List<AuthenticationSchemeProvider>
        {
            AuthenticationSchemeProviderBuilder.Create("facebook", "Facebook", typeof(FacebookHandler), new FacebookOptionsLite
            {
                AppId = "569242033233529",
                AppSecret = "12e0f33817634c0a650c0121d05e53eb"
            }).Build()
        };
    }
}
