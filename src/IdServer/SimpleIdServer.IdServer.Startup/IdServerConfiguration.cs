// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using SimpleIdServer.IdServer.Startup.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.Startup
{
    public class IdServerConfiguration
    {
        private static AuthenticationSchemeProviderDefinition Facebook = AuthenticationSchemeProviderDefinitionBuilder.Create("facebook", "Facebook", typeof(FacebookHandler), typeof(FacebookOptionsLite)).Build();

        private static IdentityProvisioningDefinition Scim = IdentityProvisioningDefinitionBuilder.Create<SCIMRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Jobs.SCIMRepresentationsExtractionJob.NAME, "SCIM")
            .AddUserSubjectMappingRule("$.userName")
            .AddUserPropertyMappingRule("$.name.familyName", nameof(User.Lastname))
            .AddUserAttributeMappingRule("$.name.givenName", JwtRegisteredClaimNames.GivenName).Build();

        private static IdentityProvisioningDefinition Ldap = IdentityProvisioningDefinitionBuilder.Create<LDAPRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Jobs.LDAPRepresentationsExtractionJob.NAME, "LDAP")
            .AddUserSubjectMappingRule("cn")
            .AddLDAPDistinguishedName()
            .Build();

        public static ICollection<Scope> Scopes => new List<Scope>
        {
            SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope,
            SimpleIdServer.IdServer.Constants.StandardScopes.Profile,
            SimpleIdServer.IdServer.Constants.StandardScopes.SAMLProfile,
            SimpleIdServer.IdServer.Constants.StandardScopes.GrantManagementQuery,
            SimpleIdServer.IdServer.Constants.StandardScopes.GrantManagementRevoke,
            SimpleIdServer.IdServer.Constants.StandardScopes.Users,
            SimpleIdServer.IdServer.Constants.StandardScopes.Register,
            SimpleIdServer.IdServer.Constants.StandardScopes.Role
        };

        public static ICollection<User> Users => new List<User>
        {
            UserBuilder.Create("administrator", "password", "Administrator").SetFirstname("Administrator").SetEmail("adm@email.com").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").GenerateRandomTOTPKey().Build(),
            UserBuilder.Create("user", "password", "User").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").Build()
        };

        public static ICollection<Client> Clients => new List<Client>
        {
            ClientBuilder.BuildTraditionalWebsiteClient("website", "password", null, "https://localhost:5001/signin-oidc", "https://localhost.com:5001/signin-oidc").AddAuthDataTypes("photo").SetClientName("Website").SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png").AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("SIDS-manager", "password", null, "http://localhost:5002/signin-oidc").AddPostLogoutUri("http://localhost:5002/signout-callback-oidc").AddAuthDataTypes("photo").SetClientName("SimpleIdServer manager").SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png").AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile).Build(),
            WsClientBuilder.BuildWsFederationClient("urn:website").SetClientName("NAME").Build(),
            ClientBuilder.BuildUserAgentClient("oauth", "password", null, "https://oauth.tools/callback/code").AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fapi", "password", null, "https://localhost:8443/test/(.*)").SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256).SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256).SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256).AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile).UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").AddSigningKey(new SigningCredentials(SimpleIdServer.IdServer.PemImporter.Import(new SimpleIdServer.IdServer.PemResult("-----BEGIN PUBLIC KEY-----\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIe\nsCLiWObLDFKKf3gJl0hll/ZTI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END PUBLIC KEY-----", "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIDHtu+N0u38ZN7DF/TpycDfaUs8WfPGUB3UusR0uv3TVoAoGCCqGSM49\nAwEHoUQDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIesCLiWObLDFKKf3gJl0hll/ZT\nI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END EC PRIVATE KEY-----"), "keyId"), SecurityAlgorithms.EcdsaSha256), SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA).Build(),
            ClientBuilder.BuildApiClient("managementClient", "password").AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.Users).AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.Register).Build()
        };

        public static ICollection<UMAResource> Resources = new List<UMAResource>
        {
            UMAResourceBuilder.Create("picture", "read", "write").SetName("Picture").Build()
        };

        public static ICollection<UMAPendingRequest> PendingRequests = new List<UMAPendingRequest>
        {
            UMAPendingRequestBuilder.Create(Guid.NewGuid().ToString(), "user", "administrator", Resources.First()).Build()
        };

        public static ICollection<AuthenticationSchemeProviderDefinition> ProviderDefinitions => new List<AuthenticationSchemeProviderDefinition>
        {
            Facebook
        };

        public static ICollection<AuthenticationSchemeProvider> Providers => new List<AuthenticationSchemeProvider>
        {
           AuthenticationSchemeProviderBuilder.Create(Facebook, "Facebook", "Facebook", "Faceoobk", new FacebookOptionsLite
           {
               AppId = "569242033233529",
               AppSecret = "12e0f33817634c0a650c0121d05e53eb"
           }).Build()
        };

        public static ICollection<Realm> Realms = new List<Realm>
        {
            SimpleIdServer.IdServer.Constants.StandardRealms.Master
        };

        public static ICollection<CertificateAuthority> CertificateAuthorities = new List<CertificateAuthority>
        {
            CertificateAuthorityBuilder.Create("CN=simpleIdServerCA", SimpleIdServer.IdServer.Constants.StandardRealms.Master).Build()
        };

        public static ICollection<IdentityProvisioningDefinition> IdentityProvisioningDefLst = new List<IdentityProvisioningDefinition>
        {
            Scim
        };

        public static ICollection<IdentityProvisioning> GetIdentityProvisiongLst(string scimEdp) => new List<IdentityProvisioning>
        {
            IdentityProvisioningBuilder.Create(Scim, "SCIM", "SCIM", new SCIMRepresentationsExtractionJobOptions
            {
                Count = 1,
                SCIMEdp = scimEdp,
                AuthenticationType = ClientAuthenticationTypes.APIKEY,
                ApiKey = "ba521b3b-02f7-4a37-b03c-58f713bf88e7"
            }).Build(),
            IdentityProvisioningBuilder.Create(Ldap, "LDAP", "LDAP", new LDAPRepresentationsExtractionJobOptions
            {
                BatchSize = 1,
                BindCredentials = "password",
                BindDN = "cn=admin,dc=xl,dc=com",
                Server = "localhost",
                Port = 389,
                UserObjectClasses = "organizationalPerson,person",
                UsersDN = "ou=people,dc=xl,dc=com"
            }).Build()
        };
    }
}
