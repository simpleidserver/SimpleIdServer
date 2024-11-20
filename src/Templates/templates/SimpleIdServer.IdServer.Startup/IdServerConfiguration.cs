﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Provisioning.LDAP;
using SimpleIdServer.IdServer.Provisioning.SCIM;
using SimpleIdServer.IdServer.Startup.Converters;
using SimpleIdServer.OpenidFederation.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Startup
{
    public class IdServerConfiguration
    {
        private static AuthenticationSchemeProviderDefinition Facebook = AuthenticationSchemeProviderDefinitionBuilder.Create("facebook", "Facebook", typeof(FacebookHandler), typeof(FacebookOptionsLite)).Build();
        private static AuthenticationSchemeProviderDefinition Google = AuthenticationSchemeProviderDefinitionBuilder.Create("google", "Google", typeof(GoogleHandler), typeof(GoogleOptionsLite)).Build();
        
        private static IdentityProvisioningDefinition Scim = IdentityProvisioningDefinitionBuilder.Create<SCIMRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Provisioning.SCIM.Services.SCIMProvisioningService.NAME, "SCIM")
            .AddUserSubjectMappingRule("$.userName")
            .AddUserPropertyMappingRule("$.name.familyName", nameof(User.Lastname))
            .AddUserAttributeMappingRule("$.name.givenName", JwtRegisteredClaimNames.GivenName)
            .AddGroupNameMappingRule("$.displayName")
            .Build();

        private static IdentityProvisioningDefinition Ldap = IdentityProvisioningDefinitionBuilder.Create<LDAPRepresentationsExtractionJobOptions>(SimpleIdServer.IdServer.Provisioning.LDAP.Services.LDAPProvisioningService.NAME, "LDAP")
            .AddUserSubjectMappingRule("cn")
            .AddGroupNameMappingRule("cn")
            .AddLDAPDistinguishedName()
            .Build();

        public static ICollection<RegistrationWorkflow> RegistrationWorkflows => new List<RegistrationWorkflow>
        {
            RegistrationWorkflowBuilder.New("pwd", true).AddStep("pwd").Build(),
            RegistrationWorkflowBuilder.New("pwd-email").AddStep("pwd").AddStep("email").Build(),
            RegistrationWorkflowBuilder.New("vp").AddStep("vp").Build(),
            RegistrationWorkflowBuilder.New("mobile").AddStep("mobile").Build()
        };

        public static Scope UniversityDegreeScope = new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = "university_degree",
            Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
            {
                StandardRealms.Master
            },
            Type = ScopeTypes.APIRESOURCE,
            Protocol = ScopeProtocols.OAUTH,
            IsExposedInConfigurationEdp = true,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };

        public static Scope CtWalletScope = new Scope
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ct_wallet",
            Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
            {
                StandardRealms.Master
            },
            Type = ScopeTypes.APIRESOURCE,
            Protocol = ScopeProtocols.OAUTH,
            IsExposedInConfigurationEdp = true,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };

        public static ICollection<Scope> Scopes => new List<Scope>
        {
            SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope,
            SimpleIdServer.IdServer.Constants.StandardScopes.Profile,
            SimpleIdServer.IdServer.Constants.StandardScopes.SAMLProfile,
            SimpleIdServer.IdServer.Constants.StandardScopes.GrantManagementQuery,
            SimpleIdServer.IdServer.Constants.StandardScopes.GrantManagementRevoke,
            SimpleIdServer.IdServer.Constants.StandardScopes.Users,
            SimpleIdServer.IdServer.Constants.StandardScopes.Register,
            SimpleIdServer.IdServer.Constants.StandardScopes.Provisioning,
            SimpleIdServer.IdServer.Constants.StandardScopes.Address,
            SimpleIdServer.IdServer.Constants.StandardScopes.Role,
            SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationSchemeProviders,
            SimpleIdServer.IdServer.Constants.StandardScopes.RegistrationWorkflows,
            SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationMethods,
            SimpleIdServer.Configuration.Constants.ConfigurationsScope,
            SimpleIdServer.IdServer.Constants.StandardScopes.ApiResources,
            SimpleIdServer.IdServer.Constants.StandardScopes.Auditing,
            SimpleIdServer.IdServer.Constants.StandardScopes.Scopes,
            SimpleIdServer.IdServer.Constants.StandardScopes.CertificateAuthorities,
            SimpleIdServer.IdServer.Constants.StandardScopes.Clients,
            SimpleIdServer.IdServer.Constants.StandardScopes.Realms,
            SimpleIdServer.IdServer.Constants.StandardScopes.Groups,
            SimpleIdServer.IdServer.Constants.StandardScopes.OfflineAccessScope,
            SimpleIdServer.IdServer.Constants.StandardScopes.CredentialConfigurations,
            SimpleIdServer.IdServer.Constants.StandardScopes.CredentialInstances,
            SimpleIdServer.IdServer.Constants.StandardScopes.DeferredCreds,
            UniversityDegreeScope,
            CtWalletScope,
            SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities,
            SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole
        };

        public static ICollection<User> Users => new List<User>
        {
            SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorUser,
            SimpleIdServer.IdServer.Constants.StandardUsers.AdministratorReadonlyUser,
            UserBuilder.Create("user", "password", "User").SetPicture("https://cdn-icons-png.flaticon.com/512/149/149071.png").Build()
        };

        public static ICollection<Client> Clients => new List<Client>
        {
            ClientBuilder.BuildWalletClient("walletClient", "password")
                .SetClientName("Wallet")
                .Build(),
            ClientBuilder.BuildCredentialIssuer("CredentialIssuer", "password", null, "https://6991-81-246-134-116.ngrok-free.app/signin-oidc", "https://localhost:5005/*", "http://localhost:5005/*", "https://credentialissuer.simpleidserver.com/*", "https://credentialissuer.localhost.com/*", "https://credentialissuer.sid.svc.cluster.local/*")
                .SetClientName("Credential issuer")
                .AddScope(
                    SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Profile,
                    UniversityDegreeScope,
                    CtWalletScope).IsTransactionCodeRequired().Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("CredentialIssuer-manager", "password", null, "https://localhost:5006/*", "https://credentialissuerwebsite.simpleidserver.com/*", "https://credentialissuerwebsite.localhost.com/*", "http://credentialissuerwebsite.localhost.com/*", "https://credentialissuerwebsite.sid.svc.cluster.local/*").EnableClientGrantType().SetRequestObjectEncryption().AddPostLogoutUri("https://localhost:5006/signout-callback-oidc").AddPostLogoutUri("https://credissuer-website.sid.svc.cluster.local/signout-callback-oidc")
                .AddPostLogoutUri("https://website.simpleidserver.com/signout-callback-oidc")
                .AddAuthDataTypes("photo")
                .SetClientName("Credential issuer manager")
                .SetBackChannelLogoutUrl("https://localhost:5006/bc-logout")
                .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
                .AddScope(
                    SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Profile,
                    SimpleIdServer.IdServer.Constants.StandardScopes.CredentialConfigurations,
                    SimpleIdServer.IdServer.Constants.StandardScopes.CredentialInstances,
                    SimpleIdServer.IdServer.Constants.StandardScopes.DeferredCreds).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("SIDS-manager", "password", null, "https://localhost:5002/*", "https://website.simpleidserver.com/*", "https://website.localhost.com/*", "http://website.localhost.com/*", "https://website.sid.svc.cluster.local/*").EnableClientGrantType().SetRequestObjectEncryption().AddPostLogoutUri("https://localhost:5002/signout-callback-oidc").AddPostLogoutUri("https://website.sid.svc.cluster.local/signout-callback-oidc")
                .AddPostLogoutUri("https://website.simpleidserver.com/signout-callback-oidc")
                .AddAuthDataTypes("photo")
                .SetClientName("SimpleIdServer manager")
                .SetBackChannelLogoutUrl("https://localhost:5002/bc-logout")
                .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
                .AddScope(
                    SimpleIdServer.IdServer.Constants.StandardScopes.Role,
                    SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.Profile, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.Provisioning,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Users, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.Acrs,
                    SimpleIdServer.Configuration.Constants.ConfigurationsScope, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationSchemeProviders, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationMethods, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.RegistrationWorkflows,
                    SimpleIdServer.IdServer.Constants.StandardScopes.ApiResources,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Auditing,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Scopes, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.CertificateAuthorities,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Clients,
                    SimpleIdServer.IdServer.Constants.StandardScopes.Realms, 
                    SimpleIdServer.IdServer.Constants.StandardScopes.Groups,
                    SimpleIdServer.IdServer.Constants.StandardScopes.WebsiteAdministratorRole,
                    SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("swaggerClient", "password", null, "https://localhost:5001/swagger/oauth2-redirect.html", "https://localhost:5001/(.*)/swagger/oauth2-redirect.html", "http://localhost").AddScope(
                SimpleIdServer.IdServer.Constants.StandardScopes.Provisioning, 
                SimpleIdServer.IdServer.Constants.StandardScopes.Users, 
                SimpleIdServer.IdServer.Constants.StandardScopes.Acrs, 
                SimpleIdServer.Configuration.Constants.ConfigurationsScope,
                SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationSchemeProviders,
                SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationMethods, 
                SimpleIdServer.IdServer.Constants.StandardScopes.RegistrationWorkflows, 
                SimpleIdServer.IdServer.Constants.StandardScopes.ApiResources, 
                SimpleIdServer.IdServer.Constants.StandardScopes.Auditing, 
                SimpleIdServer.IdServer.Constants.StandardScopes.Scopes, 
                SimpleIdServer.IdServer.Constants.StandardScopes.CertificateAuthorities,
                SimpleIdServer.IdServer.Constants.StandardScopes.Clients, 
                SimpleIdServer.IdServer.Constants.StandardScopes.Realms,
                SimpleIdServer.IdServer.Constants.StandardScopes.Groups,
                SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("postman", "password", null, "http://localhost").EnableClientGrantType().AddScope(
                SimpleIdServer.IdServer.Constants.StandardScopes.Provisioning,
                SimpleIdServer.IdServer.Constants.StandardScopes.Users,
                SimpleIdServer.IdServer.Constants.StandardScopes.Acrs,
                SimpleIdServer.Configuration.Constants.ConfigurationsScope,
                SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationSchemeProviders,
                SimpleIdServer.IdServer.Constants.StandardScopes.AuthenticationMethods,
                SimpleIdServer.IdServer.Constants.StandardScopes.RegistrationWorkflows,
                SimpleIdServer.IdServer.Constants.StandardScopes.ApiResources,
                SimpleIdServer.IdServer.Constants.StandardScopes.Auditing,
                SimpleIdServer.IdServer.Constants.StandardScopes.Scopes,
                SimpleIdServer.IdServer.Constants.StandardScopes.CertificateAuthorities,
                SimpleIdServer.IdServer.Constants.StandardScopes.Clients,
                SimpleIdServer.IdServer.Constants.StandardScopes.Realms,
                SimpleIdServer.IdServer.Constants.StandardScopes.Groups,
                SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities).Build(),
            WsClientBuilder.BuildWsFederationClient("urn:website").SetClientName("NAME").Build(),
            ClientBuilder.BuildUserAgentClient("oauth", "password", null, "https://oauth.tools/callback/code")
                .AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile)
                .Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fapi", "password", null, "https://localhost:8443/test/(.*)")
                .SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256)
                .SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256)
                .SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256)
                .AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile)
                .UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE")
                .AddSigningKey(new SigningCredentials(SimpleIdServer.IdServer.PemImporter.Import(new SimpleIdServer.IdServer.PemResult("-----BEGIN PUBLIC KEY-----\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIe\nsCLiWObLDFKKf3gJl0hll/ZTI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END PUBLIC KEY-----", "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIDHtu+N0u38ZN7DF/TpycDfaUs8WfPGUB3UusR0uv3TVoAoGCCqGSM49\nAwEHoUQDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIesCLiWObLDFKKf3gJl0hll/ZT\nI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END EC PRIVATE KEY-----"), "keyId"), SecurityAlgorithms.EcdsaSha256), SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA)
                .Build(),
            ClientBuilder.BuildApiClient("managementClient", "password").AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.Users).AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.Register).Build(),
            ClientBuilder.BuildDeviceClient("deviceClient", "password").AddScope(SimpleIdServer.IdServer.Constants.StandardScopes.OpenIdScope, SimpleIdServer.IdServer.Constants.StandardScopes.Profile).Build(),
            SamlSpClientBuilder.BuildSamlSpClient("samlSp", "http://localhost:5125/Metadata").Build()
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
            Facebook,
            Google
        };

        public static ICollection<Language> Languages => new List<Language>
        {
            LanguageBuilder.Build(Language.Default).AddDescription("English", "en").Build()
        };

        public static ICollection<AuthenticationSchemeProvider> Providers => new List<AuthenticationSchemeProvider>
        {
           AuthenticationSchemeProviderBuilder.Create(Facebook, "Facebook", "Facebook", "Facebook").Build(),
           AuthenticationSchemeProviderBuilder.Create(Google, "Google", "Google", "Google").Build(),
        };

        public static ICollection<SimpleIdServer.IdServer.Domains.Realm> Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
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
            IdentityProvisioningBuilder.Create(Scim, "SCIM", "SCIM").Build(),
            IdentityProvisioningBuilder.Create(Ldap, "LDAP", "LDAP", "LDAP").Build()
        };

        public static List<GotifySession> Sessions = new List<GotifySession>
        {
            new GotifySession { ApplicationToken = "AvSdAw5ILVOdc7g", ClientToken = "CY2St_LANPO5L7P" },
            new GotifySession { ApplicationToken = "ADIeCkMigAnGLmq", ClientToken = "C9M4RGtX.OlYD1q" }
        };

        public static List<PresentationDefinition> PresentationDefinitions = new List<PresentationDefinition>
        {
            PresentationDefinitionBuilder.New("universitydegree_vp", "University Degree").AddLdpVcInputDescriptor("UniversityDegree", "UniversityDegree", "UniversityDegree").Build()
        };

        public static List<FederationEntity> FederationEntities = new List<FederationEntity>
        {
            new FederationEntity
            {
                Id  = Guid.NewGuid().ToString(),
                IsSubordinate = false,
                Realm = SimpleIdServer.IdServer.Constants.DefaultRealm,
                Sub = "http://localhost:7000",
                CreateDateTime = DateTime.UtcNow
            }
        };
    }
}
