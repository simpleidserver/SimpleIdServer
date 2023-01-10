// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer;
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
            ScopeBuilder.Create("firstScope", true).Build(),
            ScopeBuilder.Create("secondScope", true).Build(),
            Constants.StandardScopes.OpenIdScope,
            Constants.StandardScopes.Profile,
            Constants.StandardScopes.Role,
            Constants.StandardScopes.Email
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
            ClientBuilder.BuildApiClient("thirteenClient", "password").AddScope("secondScope").SetTokenExpirationTimeInSeconds(-2).Build(),
            ClientBuilder.BuildUserAgentClient("fourteenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").Build()
        };

        public static List<User> Users = new List<User>
        {
            UserBuilder.Create("user", "password")
                .AddConsent("thirdClient", "secondScope")
                .AddConsent("nineClient", "secondScope")
                .AddConsent("fourteenClient", "openid", "profile", "role", "email")
                .AddSession("sessionId", DateTime.UtcNow.AddDays(2)).Build()
        };
    }
}
