// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
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
            ClientBuilder.BuildTraditionalWebsiteClient("fourthClient", "password", "http://localhost:9080").AddScope("secondScope").Build(),
            ClientBuilder.BuildApiClient("fifthClient", "password").AddScope("secondScope").AddRefreshTokenGrantType(-1).Build(),
            ClientBuilder.BuildApiClient("sixClient", "password").AddScope("secondScope").AddRefreshTokenGrantType().Build(),
            ClientBuilder.BuildApiClient("sevenClient", "password").AddScope("secondScope").UsePrivateKeyJwtAuthentication(JsonWebKeyBuilder.BuildRSA("seventClientKeyId")).Build(),
            ClientBuilder.BuildApiClient("eightClient", "ProEMLh5e_qnzdNU").AddScope("secondScope").UseClientSecretJwtAuthentication(JsonWebKeyBuilder.BuildRSA("eightClientKeyId")).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("nineClient", "password", "http://localhost:8080").AddScope("secondScope").UseClientPkceAuthentication().Build(),
            ClientBuilder.BuildApiClient("elevenClient", "password").AddScope("secondScope").UseClientSelfSignedAuthentication().AddSelfSignedCertificate("elevelClientKeyId").Build(),
            ClientBuilder.BuildApiClient("twelveClient", "password").AddScope("secondScope").UseClientTlsAuthentication("cn=selfSigned").Build(),
            ClientBuilder.BuildApiClient("thirteenClient", "password").AddScope("secondScope").SetTokenExpirationTimeInSeconds(-2).Build()
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
                    },
                    new Consent
                    {
                        Id = Guid.NewGuid().ToString(),
                        ClientId = "nineClient",
                        Scopes = new [] { "secondScope" }
                    }
                },
                Sessions = new List<UserSession>
                {
                    new UserSession
                    {
                        AuthenticationDateTime = DateTime.UtcNow,
                        ExpirationDateTime = DateTime.UtcNow.AddDays(2),
                        SessionId = "sessionid",
                        State = UserSessionStates.Active
                    }
                }
            }
        };
    }
}
