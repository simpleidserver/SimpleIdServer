// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Builders;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class IdServerConfiguration
    {
        public static List<Scope> Scopes => new List<Scope>
        {
            new Scope
            {
                Name = "firstScope",
                IsExposedInConfigurationEdp = true,
            },
            new Scope
            {
                Name = "secondScope",
                IsExposedInConfigurationEdp = true
            }
        };

        public static List<Client> Clients = new List<Client>
        {
            ClientBuilder.BuildApiClient("firstClient", "password").AddScope("firstScope").Build(),
            ClientBuilder.BuildApiClient("secondClient", "password").AddScope("firstScope").UseOnlyPasswordGrantType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirdClient", "password", "http://localhost:8080").AddScope("secondScope").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fourthClient", "password", "http://localhost:9080").AddScope("secondScope").Build()
        };

        public static List<User> Users = new List<User>
        {
            new User
            {
                Id = "user",
                Credentials = new List<UserCredential>
                {
                    UserCredential.CreatePassword("password")
                },
                Consents = new List<Consent>
                {
                    new Consent
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = "thirdClient",
                        Scopes = new [] { "secondScope" }
                    }
                }
            }
        };
    }
}
