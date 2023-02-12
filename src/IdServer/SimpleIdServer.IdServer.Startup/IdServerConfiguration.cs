// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Startup.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Startup
{
    public class IdServerConfiguration
    {

        public static ICollection<Scope> Scopes => new List<Scope>
        {
            Constants.StandardScopes.OpenIdScope,
            Constants.StandardScopes.Profile,
            Constants.StandardScopes.SAMLProfile
        };

        public static ICollection<User> Users => new List<User>
        {
            UserBuilder.Create("administrator", "password", "Administrator").SetFirstname("Administrator").SetEmail("adm@email.com").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").GenerateRandomTOTPKey().Build(),
            UserBuilder.Create("user", "password", "User").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").Build()
        };

        public static ICollection<Client> Clients => new List<Client>
        {
            ClientBuilder.BuildTraditionalWebsiteClient("website", "password", "http://localhost:60001/signin-oidc").SetClientName("Website").SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png").AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).EnableWsFederationProtocol().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("bankWebsite", "password", "http://localhost:60001/signin-oidc").SetClientName("Bank Website").AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).EnableCIBAGrantType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("urn:website", "password").AddScope(Constants.StandardScopes.SAMLProfile).SetClientName("Name").EnableWsFederationProtocol().Build()
        };

        public static ICollection<UMAResource> Resources = new List<UMAResource>
        {
            UMAResourceBuilder.Create("picture", "read", "write").SetName("Picture").Build()
        };

        public static ICollection<UMAPendingRequest> PendingRequests = new List<UMAPendingRequest>
        {
            UMAPendingRequestBuilder.Create(Guid.NewGuid().ToString(), "user", "administrator", Resources.First()).Build()
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
