// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.OpenidFederation.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.OAuth.Host.Acceptance.Tests;

public class IdServerConfiguration
{
    public static Scope GetRoleScope()
    {
        var result = Constants.StandardScopes.Role;
        var claimMapper = result.ClaimMappers.ElementAt(0);
        claimMapper.IncludeInAccessToken = true;
        return result;
    }

    public static Dictionary<string, SigningCredentials> ClientSigningCredentials = new Dictionary<string, SigningCredentials>
    {
        { "sevenClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "eightClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "thirtyOneClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "thirtyTwoClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "thirtyThreeClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "fortyTwoClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "fortyThreeClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256) },
        { "fortyFourClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256)  },
        { "sixtySixClient", new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaSha256)  }
    };

    public static SigningCredentials RpSigningCredential = new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "rpKeyId" }, SecurityAlgorithms.RsaSha256);

    public static SigningCredentials RpJwtSigningCredential = new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "rpJwtKeyId" }, SecurityAlgorithms.RsaSha256);

    public static Dictionary<string, EncryptingCredentials> ClientEncryptingCredentials = new Dictionary<string, EncryptingCredentials>
    {
        { "twentyFiveClient", new EncryptingCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256) },
        { "twentySixClient", new EncryptingCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384) },
        { "twentySevenClient", new EncryptingCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512) },
        { "twentyEightClient", new EncryptingCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256) },
        { "twentyNineClient", new EncryptingCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384) }
    };

    private static Client FiftySixClient = ClientBuilder.BuildUserAgentClient("fiftySixClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role).Build();
    private static Scope FiftySixClientAdminRole = ScopeBuilder.CreateRoleScope(FiftySixClient, "admin", "admin").Build();
    private static Scope FirstScope = ScopeBuilder.CreateApiScope("firstScope", true).Build();
    private static Scope SecondScope = ScopeBuilder.CreateApiScope("secondScope", true).Build();
    private static Scope AdminScope = ScopeBuilder.CreateApiScope("admin", true).Build();
    private static Scope CalendarScope = ScopeBuilder.CreateApiScope("calendar", true).Build();
    private static Scope UniversityCredential = ScopeBuilder.CreateApiScope("UniversityCredential", true).Build();
    private static Group AdminGroup = GroupBuilder.Create("admin", "admin").AddRealm(SimpleIdServer.IdServer.Constants.DefaultRealm).AddRole(FiftySixClientAdminRole).Build();
    private static User OtpUser = UserBuilder.Create("otpUser", "password")
        .GenerateRandomTOTPKey()
        .Build();
    private static User User = UserBuilder.Create("user", "password")
            .SetEmail("email@outlook.fr")
            .AddRole("role1")
            .AddRole("role2")
            .AddGroup(AdminGroup)
            .SetAddress("street", "locality", "region", "postalcode", "country", "formatted")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirdClient", "secondScope")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "nineClient", "secondScope")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortyTwoClient", "secondScope")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fourteenClient", "openid", "profile", "role", "email", "address")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fifteenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "sixteenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "seventeenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "eighteenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "nineteenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyOneClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyTwoClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyThreeClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyFourClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyFiveClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentySixClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentySevenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyEightClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "twentyNineClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyOneClient", new[] { "openid", "profile", "role", "email" }, new[] { "sub" })
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyTwoClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyThreeClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyFourClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyFiveClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtySixClient", "openid", "profile", "role", "email", "offline_access")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtySevenClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyEightClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "thirtyNineClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortyClient", new string[] { "openid", "profile", "role", "email" }, new string[] { "name", "email" })
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortyOneClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortySixClient", "admin", "calendar", "offline_access")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortySevenClient", new string[] { "admin", "calendar", "grant_management_query", "grant_management_revoke", "offline_access" }, new AuthorizationData[] { new AuthorizationData { Type = "firstDetails", Actions = new List<string> { "read" } }, new AuthorizationData { Type = "secondDetails", Locations = new List<string> { "https://cal.example.com" }, Actions = new List<string> { "read" } } }, "consentId")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortyEightClient", "admin", "calendar", "grant_management_query", "grant_management_revoke")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortyNineClient", "admin", "calendar")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fortyNineClient", new AuthorizationData { Type = "firstDetails", Actions = new List<string> { "read" } }, new AuthorizationData { Type = "secondDetails", Locations = new List<string> { "https://cal.example.com" }, Actions = new List<string> { "read" } })
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftyClient", "admin", "calendar")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftyOneClient", "admin", "calendar")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftyTwoClient", "admin", "calendar")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftyFourClient", "openid", "profile", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftyFiveClient", new AuthorizationData { Type = "firstDetails", Actions = new List<string> { "read" } }, new AuthorizationData { Type = "secondDetails", Locations = new List<string> { "https://cal.example.com" }, Actions = new List<string> { "read" } })
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftySixClient", "openid", "role")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "fiftyEightClient", new AuthorizationData
            {
                Type = "openid_credential",
                AdditionalData = new Dictionary<string, string>
                {
                    { "types", JsonSerializer.Serialize(new List<string> { "UniversityDegreeCredential" }) }
                }
            })
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "sixtyFiveClient", "secondScope")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "seventyClient", "openid", "profile", "role", "email")
            .AddConsent(SimpleIdServer.IdServer.Constants.DefaultRealm, "seventyFourClient", "openid", "profile")
            .AddClaim("degreeType", "BachelorDegree")
            .AddClaim("degreeName", "Bachelor of Science and Arts")
            .Build();
    private static UserSession UserSession = new UserSession { SessionId = "sessionId", AuthenticationDateTime = DateTime.UtcNow, ExpirationDateTime = DateTime.UtcNow.AddDays(2), State = UserSessionStates.Active, Realm = SimpleIdServer.IdServer.Constants.DefaultRealm, UserId = User.Id };

    public static List<Group> Groups => new List<Group>
    {
        AdminGroup
    };

    public static List<UserSession> Sessions = new List<UserSession>
    {
        UserSession
    };

    public static List<Scope> Scopes => new List<Scope>
    {
        FirstScope,
        SecondScope,
        AdminScope,
        CalendarScope,
        UniversityCredential,
        Constants.StandardScopes.OpenIdScope,
        Constants.StandardScopes.Profile,
        GetRoleScope(),
        Constants.StandardScopes.Email,
        Constants.StandardScopes.OfflineAccessScope,
        Constants.StandardScopes.GrantManagementQuery,
        Constants.StandardScopes.GrantManagementRevoke,
        Constants.StandardScopes.Users,
        Constants.StandardScopes.Register,
        Constants.StandardScopes.Provisioning,
        Constants.StandardScopes.Acrs
    };

    public static List<ApiResource> ApiResources = new List<ApiResource>
    {
        ApiResourceBuilder.Create("https://cal.example.com",  "https://cal.example.com", "description", AdminScope, CalendarScope).Build(),
        ApiResourceBuilder.Create("https://contacts.example.com", "https://contacts.example.com", "description", AdminScope, CalendarScope).Build(),
        ApiResourceBuilder.Create("firstClient", "firstClient", "First API", FirstScope).Build(),
        ApiResourceBuilder.Create("secondClient", "secondClient", "Second API", FirstScope).Build(),
    };

    public static List<Client> Clients = new List<Client>
        {
            ClientBuilder.BuildApiClient("firstClient", "password").AddScope(FirstScope).Build(),
            ClientBuilder.BuildApiClient("secondClient", "password").AddScope(FirstScope).UseOnlyPasswordGrantType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirdClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(SecondScope).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fourthClient", "password", null, "http://localhost:9080").AddScope(SecondScope).Build(),
            ClientBuilder.BuildApiClient("fifthClient", "password").AddScope(SecondScope).EnableRefreshTokenGrantType(-1).Build(),
            ClientBuilder.BuildApiClient("sixClient", "password").AddScope(SecondScope).EnableRefreshTokenGrantType().Build(),
            ClientBuilder.BuildApiClient("sevenClient", "password").AddScope(SecondScope).AddSigningKey(ClientSigningCredentials["sevenClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).UsePrivateKeyJwtAuthentication().Build(),
            ClientBuilder.BuildApiClient("eightClient", "ProEMLh5e_qnzdNU").AddScope(SecondScope).AddSigningKey(ClientSigningCredentials["eightClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).UseClientSecretJwtAuthentication().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("nineClient", "password",null, "http://localhost:8080").AddScope(SecondScope).UseClientPkceAuthentication().Build(),
            ClientBuilder.BuildApiClient("elevenClient", "password").AddScope(SecondScope).UseClientSelfSignedAuthentication().AddSelfSignedCertificate("elevelClientKeyId").Build(),
            ClientBuilder.BuildApiClient("twelveClient", "password").AddScope(SecondScope).UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").Build(),
            ClientBuilder.BuildApiClient("thirteenClient", "password").AddScope(SecondScope).SetTokenExpirationTimeInSeconds(-2).Build(),
            ClientBuilder.BuildUserAgentClient("fourteenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email, StandardScopes.Address).Build(),
            ClientBuilder.BuildUserAgentClient("fifteenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).DisableIdTokenSignature().Build(),
            ClientBuilder.BuildUserAgentClient("sixteenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha256).Build(),
            ClientBuilder.BuildUserAgentClient("seventeenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha384).Build(),
            ClientBuilder.BuildUserAgentClient("eighteenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.EcdsaSha512).Build(),
            ClientBuilder.BuildUserAgentClient("nineteenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentyOneClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.HmacSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyTwoClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildUserAgentClient("twentyThreeClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha384).Build(),
            ClientBuilder.BuildUserAgentClient("twentyFourClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenSignatureAlg(SecurityAlgorithms.RsaSha512).Build(),
            ClientBuilder.BuildUserAgentClient("twentyFiveClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).AddEncryptedKey(ClientEncryptingCredentials["twentyFiveClient"], SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildUserAgentClient("twentySixClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384).AddEncryptedKey(ClientEncryptingCredentials["twentySixClient"], SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildUserAgentClient("twentySevenClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512).AddEncryptedKey(ClientEncryptingCredentials["twentySevenClient"], SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildUserAgentClient("twentyEightClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256).AddEncryptedKey(ClientEncryptingCredentials["twentyEightClient"], SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildUserAgentClient("twentyNineClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384).AddEncryptedKey(ClientEncryptingCredentials["twentyNineClient"], SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildUserAgentClient("thirtyClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetIdTokenEncryption(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512).AddRSAEncryptedKey(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512, SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyOneClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).EnableIdTokenInResponseType().AddSigningKey(ClientSigningCredentials["thirtyOneClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyTwoClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).SetRequestObjectEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).EnableIdTokenInResponseType().AddSigningKey(ClientSigningCredentials["thirtyTwoClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyThreeClient", "password", null, "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.RsaSha256).SetRequestObjectEncryption(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).EnableIdTokenInResponseType().AddSigningKey(ClientSigningCredentials["thirtyThreeClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).SetPairwiseSubjectType("salt").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyFourClient", "password", null, "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetDefaultMaxAge(2).EnableIdTokenInResponseType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyFiveClient", "password", null, "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).EnableIdTokenInResponseType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtySixClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email, StandardScopes.OfflineAccessScope).EnableOfflineAccess().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtySevenClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyEightClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetUserInfoSignedResponseAlg().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("thirtyNineClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetUserInfoSignedResponseAlg().SetUserInfoEncryption().AddRSAEncryptedKey(new RsaSecurityKey(RSA.Create()) { KeyId = "keyId" }, SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyOneClient", "password", null, "http://localhost:8080").AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha384).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyTwoClient", "password", null, "http://localhost:8080").AddScope(SecondScope).AddAuthDataTypes("firstDetails").EnableCIBAGrantType(StandardNotificationModes.Ping, "http://localhost/notificationedp").UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").AddSigningKey(ClientSigningCredentials["fortyTwoClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyThreeClient", "password", null, "http://localhost:8080").AddScope(SecondScope).EnableCIBAGrantType(StandardNotificationModes.Push, "http://localhost/notificationedp").UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").AddSigningKey(ClientSigningCredentials["fortyThreeClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyFourClient", "password", null, "http://localhost:8080").AddScope(SecondScope).EnableCIBAGrantType(StandardNotificationModes.Poll).UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").AddSigningKey(ClientSigningCredentials["fortyFourClient"], SecurityAlgorithms.RsaSha256, SecurityKeyTypes.RSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortySixClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().EnableTokenInResponseType().EnableRefreshTokenGrantType().ResourceParameterIsRequired().AddScope(AdminScope, CalendarScope, Constants.StandardScopes.OfflineAccessScope).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortySevenClient", "password", null, "http://localhost:8080").AddAuthDataTypes("secondDetails").UseClientSecretPostAuthentication().EnableTokenInResponseType().EnableRefreshTokenGrantType().AddScope(AdminScope, CalendarScope, Constants.StandardScopes.OfflineAccessScope).EnableAccessToGrantsApi().DisableConsent().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyEightClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().EnableTokenInResponseType().EnableRefreshTokenGrantType().AddScope(AdminScope, CalendarScope).EnableAccessToGrantsApi().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fortyNineClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(AdminScope, CalendarScope).AddAuthDataTypes("secondDetails").EnableCIBAGrantType(StandardNotificationModes.Poll, interval: 0).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Poll).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyOneClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Push, "http://localhost/notificationedp").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyTwoClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().AddScope(AdminScope, CalendarScope).EnableCIBAGrantType(StandardNotificationModes.Ping, "http://localhost/notificationedp", 0).Build(),
            ClientBuilder.BuildApiClient("fiftyThreeClient", "password").EnableUMAGrantType().ActAsUMAResourceServer().Build(),
            ClientBuilder.BuildUserAgentClient("fiftyFourClient", "password", null, "http://localhost:8080").UseImplicitFlow().AddScope(StandardScopes.OpenIdScope, StandardScopes.Profile, StandardScopes.Email).SetSigAuthorizationResponse(SecurityAlgorithms.RsaSha256).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyFiveClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().EnableTokenInResponseType().EnableRefreshTokenGrantType().AddScope(AdminScope, CalendarScope).AddAuthDataTypes("firstDetails", "secondDetails").Build(),
            FiftySixClient,
            ClientBuilder.BuildApiClient("fiftySevenClient", "password").AddScope(StandardScopes.Users).AddScope(StandardScopes.Register).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fiftyEightClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().EnableTokenInResponseType().AddAuthDataTypes("openid_credential").DisableConsent().AddScope(StandardScopes.GrantManagementQuery, StandardScopes.GrantManagementRevoke).Build(),
            ClientBuilder.BuildWalletClient("fiftyNineClient", "password").Build(),
            ClientBuilder.BuildWalletClient("sixtyClient", "password").RequireTransactionCode().Build(),
            ClientBuilder.BuildDeviceClient("sixtyOneClient", "password").AddScope(AdminScope).Build(),
            ClientBuilder.BuildApiClient("sixtyTwoClient", "password").AddScope(StandardScopes.Acrs).Build(),
            ClientBuilder.BuildApiClient("sixtyThreeClient", "password").AddScope(FirstScope).UseDPOPProof().EnableRefreshTokenGrantType().Build(),
            ClientBuilder.BuildApiClient("sixtyFourClient", "password").AddScope(FirstScope).UseDPOPProof(true).EnableRefreshTokenGrantType().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("sixtyFiveClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().UseDPOPProof(true).AddScope(SecondScope).Build(),
            ClientBuilder.BuildApiClient("sixtySixClient", "password").EnableExchangeTokenGrantType(TokenExchangeTypes.IMPERSONATION).Build(),
            ClientBuilder.BuildApiClient("sixtySevenClient", "password").EnableExchangeTokenGrantType(TokenExchangeTypes.DELEGATION).Build(),
            ClientBuilder.BuildApiClient("sixtyEightClient", "password").AddScope(FirstScope).SetAccessTokenType(AccessTokenTypes.Reference).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("sixtyNineClient", "password", null, "http://localhost:8080").UseClientSecretPostAuthentication().DisableConsent().AddScope(GetRoleScope()).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("seventyClient", "password", null, "http://localhost:8080").SetAccessTokenType(AccessTokenTypes.Reference).UseClientSecretPostAuthentication().AddScope(StandardScopes.OpenIdScope, StandardScopes.Role, StandardScopes.Profile, StandardScopes.Email).Build(),
            ClientBuilder.BuildApiClient("seventyOneClient", "password").AddScope(FirstScope).UseClientSecretBasicAuthentication().Build(),
            ClientBuilder.BuildCredentialIssuer("seventyTwoClient", "password").IsTransactionCodeRequired().AddScope(UniversityCredential).Build(),
            ClientBuilder.BuildWalletClient("seventyThreeClient", "password").Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("seventyFourClient", "password", null, "http://localhost:8080").DisableConsent().AddScope(StandardScopes.OpenIdScope, StandardScopes.Profile).Build(),
        };

    public static List<DeviceAuthCode> DeviceAuthCodes = new List<DeviceAuthCode>
    {
        new DeviceAuthCode
        {
            DeviceCode = "issuedDeviceCode",
            ClientId = "fiftyEightClient",
            Status = DeviceAuthCodeStatus.ISSUED,
            UserCode = Guid.NewGuid().ToString()
        },
        new DeviceAuthCode
        {
            DeviceCode = "expiredDeviceCode",
            ClientId = "fiftyEightClient",
            Status = DeviceAuthCodeStatus.PENDING,
            ExpirationDateTime = DateTime.UtcNow.AddHours(-2),
            UserCode = Guid.NewGuid().ToString()
        },
        new DeviceAuthCode
        {
            DeviceCode = "invalidClientDeviceCode",
            ClientId = "fiftyEightClient",
            Status = DeviceAuthCodeStatus.PENDING,
            ExpirationDateTime = DateTime.UtcNow.AddHours(2),
            UserCode = Guid.NewGuid().ToString()
        },
        new DeviceAuthCode
        {
            DeviceCode = "tooManyDeviceCode",
            ClientId = Clients.First(c => c.ClientId == "sixtyOneClient").Id,
            ExpirationDateTime = DateTime.UtcNow.AddDays(2),
            Status = DeviceAuthCodeStatus.PENDING,
            NextAccessDateTime = DateTime.UtcNow.AddDays(2),
            UserCode = Guid.NewGuid().ToString()
        },
        new DeviceAuthCode
        {
            DeviceCode = "pendingDeviceCode",
            ClientId = Clients.First(c => c.ClientId == "sixtyOneClient").Id,
            ExpirationDateTime = DateTime.UtcNow.AddDays(2),
            Status = DeviceAuthCodeStatus.PENDING,
            UserCode = Guid.NewGuid().ToString()
        }
    };

    public static List<FederationEntity> FederationEntities = new List<FederationEntity>
    {
        new FederationEntity
        {
            Id = Guid.NewGuid().ToString(),
            Realm = IdServer.Constants.DefaultRealm,
            Sub = "http://ta.com",
            IsSubordinate = false
        }
    };

    public static List<User> Users = new List<User>
    {
        User,
        OtpUser
    };

    public static List<UMAResource> UmaResources = new List<UMAResource>
    {
        UMAResourceBuilder.Create("id", "read", "write").Build()
    };



    public static List<BCAuthorize> BCAuthorizeLst = new List<BCAuthorize>
    {
        new BCAuthorize { Id = "expiredBC", ExpirationDateTime = DateTime.UtcNow.AddYears(-1), Realm = SimpleIdServer.IdServer.Constants.DefaultRealm },
        new BCAuthorize { Id = "confirmedBC", ExpirationDateTime = DateTime.UtcNow.AddYears(1), LastStatus = BCAuthorizeStatus.Confirmed, Realm = SimpleIdServer.IdServer.Constants.DefaultRealm },
        new BCAuthorize { Id = "invalidUser", ExpirationDateTime = DateTime.UtcNow.AddYears(1), LastStatus = BCAuthorizeStatus.Pending, UserId = "otherUserId", Realm = SimpleIdServer.IdServer.Constants.DefaultRealm }
    };

    public static List<SimpleIdServer.IdServer.Domains.Realm> Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
    {
        SimpleIdServer.IdServer.Constants.StandardRealms.Master
    };

    private static X509SecurityKey GenerateRandomSelfSignedCertificate()
    {
        var ecdsa = ECDsa.Create();
        var req = new CertificateRequest("cn=selfSigned", ecdsa, HashAlgorithmName.SHA256);
        var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(2));
        var key = new X509SecurityKey(cert)
        {
            KeyId = "selfSignedId"
        };
        return key;
    }
}