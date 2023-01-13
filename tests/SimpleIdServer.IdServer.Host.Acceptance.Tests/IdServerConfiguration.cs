// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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
            Constants.StandardScopes.Email,
            Constants.StandardScopes.OfflineAccessScope
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
            ClientBuilder.BuildTraditionalWebsiteClient("nineClient", "password", "http://localhost:8080").AddScope("secondScope").EnableClientPkceAuthentication().Build(),
            ClientBuilder.BuildApiClient("elevenClient", "password").AddScope("secondScope").UseClientSelfSignedAuthentication().AddSelfSignedCertificate("elevelClientKeyId").Build(),
            ClientBuilder.BuildApiClient("twelveClient", "password").AddScope("secondScope").UseClientTlsAuthentication("cn=selfSigned").Build(),
            ClientBuilder.BuildApiClient("thirteenClient", "password").AddScope("secondScope").SetTokenExpirationTimeInSeconds(-2).Build(),
            ClientBuilder.BuildUserAgentClient("fourteenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").Build(),
            ClientBuilder.BuildUserAgentClient("fifteenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").DisableIdTokenSignature().Build(),
            ClientBuilder.BuildUserAgentClient("sixteenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha256).Build(),
            ClientBuilder.BuildUserAgentClient("seventeenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha384).Build(),
            ClientBuilder.BuildUserAgentClient("eighteenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha512).Build(),
            ClientBuilder.BuildUserAgentClient("nineteenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentyOneClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyTwoClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyThreeClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentyFourClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyFiveClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentySixClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentySevenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyEightClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyNineClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("thirtyClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyOneClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).EnableIdTokenInResponseType().AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyTwoClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).SetRequestObjectEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).EnableIdTokenInResponseType().AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyThreeClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).SetRequestObjectEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).EnableIdTokenInResponseType().AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).SetPairwiseSubjectType("salt").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyFourClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetDefaultMaxAge(2).EnableIdTokenInResponseType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyFiveClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").EnableIdTokenInResponseType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtySixClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email", "offline_access").EnableOfflineAccess().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtySevenClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyEightClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetUserInfoSignedResponseAlg().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyNineClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").SetUserInfoSignedResponseAlg().SetUserInfoEncryption().AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyClient", "password", "http://localhost:8080").AddScope("openid", "role", "profile", "email").Build()
        };

        public static List<User> Users = new List<User>
        {
            UserBuilder.Create("user", "password")
                .SetEmail("email@outlook.fr")
                .AddRole("role1")
                .AddRole("role2")
                .AddConsent("thirdClient", "secondScope")
                .AddConsent("nineClient", "secondScope")
                .AddConsent("fourteenClient", "openid", "profile", "role", "email")
                .AddConsent("fifteenClient", "openid", "profile", "role", "email")
                .AddConsent("sixteenClient", "openid", "profile", "role", "email")
                .AddConsent("seventeenClient", "openid", "profile", "role", "email")
                .AddConsent("eighteenClient", "openid", "profile", "role", "email")
                .AddConsent("nineteenClient", "openid", "profile", "role", "email")
                .AddConsent("twentyClient", "openid", "profile", "role", "email")
                .AddConsent("twentyOneClient", "openid", "profile", "role", "email")
                .AddConsent("twentyTwoClient", "openid", "profile", "role", "email")
                .AddConsent("twentyThreeClient", "openid", "profile", "role", "email")
                .AddConsent("twentyFourClient", "openid", "profile", "role", "email")
                .AddConsent("twentyFiveClient", "openid", "profile", "role", "email")
                .AddConsent("twentySixClient", "openid", "profile", "role", "email")
                .AddConsent("twentySevenClient", "openid", "profile", "role", "email")
                .AddConsent("twentyEightClient", "openid", "profile", "role", "email")
                .AddConsent("twentyNineClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyOneClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyTwoClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyThreeClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyFourClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyFiveClient", "openid", "profile", "role", "email")
                .AddConsent("thirtySixClient", "openid", "profile", "role", "email", "offline_access")
                .AddConsent("thirtySevenClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyEightClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyNineClient", "openid", "profile", "role", "email")
                .AddConsent("fortyClient", new string[] { "openid", "profile", "role", "email" }, new string[] { "name", "email" })
                .AddSession("sessionId", DateTime.UtcNow.AddDays(2)).Build()
        };
    }
}
