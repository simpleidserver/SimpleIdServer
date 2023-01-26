// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests
{
    public class IdServerConfiguration
    {
        private static Scope FirstScope = ScopeBuilder.CreateApiScope("firstScope", true).Build();
        private static Scope SecondScope = ScopeBuilder.CreateApiScope("secondScope", true).Build();
        private static Scope AdminScope = ScopeBuilder.CreateApiScope("admin", true).Build();
        private static Scope CalendarScope = ScopeBuilder.CreateApiScope("calendar", true).Build();

        public static List<Scope> Scopes => new List<Scope>
        {
            FirstScope,
            SecondScope,
            AdminScope,
            CalendarScope,
            Constants.StandardScopes.OpenIdScope,
            Constants.StandardScopes.Profile,
            Constants.StandardScopes.Role,
            Constants.StandardScopes.Email,
            Constants.StandardScopes.OfflineAccessScope,
            Constants.StandardScopes.GrantManagementQuery,
            Constants.StandardScopes.GrantManagementRevoke
        };

        public static List<ApiResource> ApiResources = new List<ApiResource>
        {
            ApiResourceBuilder.Create("https://cal.example.com", "description", AdminScope, CalendarScope).Build(),
            ApiResourceBuilder.Create("https://contacts.example.com", "description", AdminScope, CalendarScope).Build(),
            ApiResourceBuilder.Create("firstClient", "First API", FirstScope).Build(),
            ApiResourceBuilder.Create("secondClient", "Second API", FirstScope).Build(),
        };

        public static List<Client> Clients = new List<Client>
        {
            ClientBuilder.BuildApiClient("firstClient", "password").AddScope(FirstScope).Build(),
            ClientBuilder.BuildApiClient("secondClient", "password").AddScope(FirstScope).UseOnlyPasswordGrantType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirdClient", "password", "http://localhost:8080").AddScope(SecondScope).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fourthClient", "password", "http://localhost:9080").AddScope(SecondScope).Build(),
            ClientBuilder.BuildApiClient("fifthClient", "password").AddScope(SecondScope).EnableRefreshTokenGrantType(-1).Build(),
            ClientBuilder.BuildApiClient("sixClient", "password").AddScope(SecondScope).EnableRefreshTokenGrantType().Build(),
            ClientBuilder.BuildApiClient("sevenClient", "password").AddScope(SecondScope).UsePrivateKeyJwtAuthentication(JsonWebKeyBuilder.BuildRSA("seventClientKeyId")).Build(),
            ClientBuilder.BuildApiClient("eightClient", "ProEMLh5e_qnzdNU").AddScope(SecondScope).UseClientSecretJwtAuthentication(JsonWebKeyBuilder.BuildRSA("eightClientKeyId")).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("nineClient", "password", "http://localhost:8080").AddScope(SecondScope).UseClientPkceAuthentication().Build(),
            ClientBuilder.BuildApiClient("elevenClient", "password").AddScope(SecondScope).UseClientSelfSignedAuthentication().AddSelfSignedCertificate("elevelClientKeyId").Build(),
            ClientBuilder.BuildApiClient("twelveClient", "password").AddScope(SecondScope).UseClientTlsAuthentication("cn=selfSigned").Build(),
            ClientBuilder.BuildApiClient("thirteenClient", "password").AddScope(SecondScope).SetTokenExpirationTimeInSeconds(-2).Build(),
            ClientBuilder.BuildUserAgentClient("fourteenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).Build(),
            ClientBuilder.BuildUserAgentClient("fifteenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).DisableIdTokenSignature().Build(),
            ClientBuilder.BuildUserAgentClient("sixteenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha256).Build(),
            ClientBuilder.BuildUserAgentClient("seventeenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha384).Build(),
            ClientBuilder.BuildUserAgentClient("eighteenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha512).Build(),
            ClientBuilder.BuildUserAgentClient("nineteenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentyOneClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyTwoClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyThreeClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentyFourClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyFiveClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentySixClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentySevenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyEightClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyNineClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("thirtyClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512).AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyOneClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).EnableIdTokenInResponseType().AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyTwoClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).SetRequestObjectEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).EnableIdTokenInResponseType().AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyThreeClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).SetRequestObjectEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).EnableIdTokenInResponseType().AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).SetPairwiseSubjectType("salt").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyFourClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetDefaultMaxAge(2).EnableIdTokenInResponseType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyFiveClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).EnableIdTokenInResponseType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtySixClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email, StandardScopes.OfflineAccessScope).EnableOfflineAccess().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtySevenClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyEightClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetUserInfoSignedResponseAlg().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyNineClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetUserInfoSignedResponseAlg().SetUserInfoEncryption().AddRSAEncryptedKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyOneClient", "password", "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha384).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyTwoClient", "password", "http://localhost:8080").AddScope(SecondScope).EnableCIBAGrantType(StandardNotificationModes.Ping, "http://localhost/notificationedp").UseClientTlsAuthentication("CN=firstMtlsClient").AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyThreeClient", "password", "http://localhost:8080").AddScope(SecondScope).EnableCIBAGrantType(StandardNotificationModes.Push, "http://localhost/notificationedp").UseClientTlsAuthentication("CN=firstMtlsClient").AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyFourClient", "password", "http://localhost:8080").AddScope(SecondScope).EnableCIBAGrantType(StandardNotificationModes.Poll).UseClientTlsAuthentication("CN=firstMtlsClient").AddSigningKey(new RsaSecurityKey(new RSACryptoServiceProvider(2048)) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortySixClient", "password", "http://localhost:8080").EnableTokenInResponseType().EnableRefreshTokenGrantType().ResourceParameterIsRequired().AddScope(AdminScope, CalendarScope).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortySevenClient", "password", "http://localhost:8080").EnableTokenInResponseType().EnableRefreshTokenGrantType().AddScope(AdminScope, CalendarScope).EnableAccessToGrantsApi().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyEightClient", "password", "http://localhost:8080").EnableTokenInResponseType().EnableRefreshTokenGrantType().AddScope(AdminScope, CalendarScope).EnableAccessToGrantsApi().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyNineClient", "password", "http://localhost:8080").AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Poll, interval: 0).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyClient", "password", "http://localhost:8080").AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Poll).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyOneClient", "password", "http://localhost:8080").AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Push, "http://localhost/notificationedp").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyTwoClient", "password", "http://localhost:8080").AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Ping, "http://localhost/notificationedp", 0).Build(),
            ClientBuilder.BuildApiClient("fiftyThreeClient", "password").ActAsUMAResourceServer().Build()
        };

        public static List<User> Users = new List<User>
        {
            UserBuilder.Create("user", "password")
                .SetEmail("email@outlook.fr")
                .AddRole("role1")
                .AddRole("role2")
                .AddConsent("thirdClient", "secondScope")
                .AddConsent("nineClient", "secondScope")
                .AddConsent("fortyTwoClient", "secondScope")
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
                .AddConsent("thirtyOneClient", new [] { "openid", "profile", "role", "email" }, new [] { "sub" })
                .AddConsent("thirtyTwoClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyThreeClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyFourClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyFiveClient", "openid", "profile", "role", "email")
                .AddConsent("thirtySixClient", "openid", "profile", "role", "email", "offline_access")
                .AddConsent("thirtySevenClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyEightClient", "openid", "profile", "role", "email")
                .AddConsent("thirtyNineClient", "openid", "profile", "role", "email")
                .AddConsent("fortyClient", new string[] { "openid", "profile", "role", "email" }, new string[] { "name", "email" })
                .AddConsent("fortyOneClient", "openid", "profile", "role", "email")
                .AddConsent("fortySixClient", "admin", "calendar")
                .AddConsent("fortySevenClient", "admin", "calendar", "grant_management_query", "grant_management_revoke")
                .AddConsent("fortyEightClient", "admin", "calendar", "grant_management_query", "grant_management_revoke")
                .AddConsent("fortyNineClient", "admin", "calendar")
                .AddConsent("fiftyClient", "admin", "calendar")
                .AddConsent("fiftyOneClient", "admin", "calendar")
                .AddConsent("fiftyTwoClient", "admin", "calendar")
                .AddSession("sessionId", DateTime.UtcNow.AddDays(2)).Build()
        };
    }
}
