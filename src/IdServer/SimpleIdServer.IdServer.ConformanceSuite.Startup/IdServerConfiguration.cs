// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.ConformanceSuite.Startup.Converters;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer.ConformanceSuite.Startup
{
    public class IdServerConfiguration
    {
        private static AuthenticationSchemeProviderDefinition Facebook = AuthenticationSchemeProviderDefinitionBuilder.Create("facebook", "Facebook", typeof(FacebookHandler), typeof(FacebookOptionsLite)).Build();

        private static IdentityProvisioningDefinition Scim = IdentityProvisioningDefinitionBuilder.Create<SyncSCIMRepresentationsOptions>(SyncSCIMRepresentationsJob.NAME, "SCIM").Build();

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
            ClientBuilder.BuildTraditionalWebsiteClient("website", "password", null, "https://localhost.com:5001/signin-oidc").SetClientName("Website").SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png").AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fapi", "password", null, "https://localhost:8443/test/(.*)").SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256).SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256).SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256).AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").AddSigningKey(new SigningCredentials(PemImporter.Import(new PemResult("-----BEGIN PUBLIC KEY-----\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIe\nsCLiWObLDFKKf3gJl0hll/ZTI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END PUBLIC KEY-----", "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIDHtu+N0u38ZN7DF/TpycDfaUs8WfPGUB3UusR0uv3TVoAoGCCqGSM49\nAwEHoUQDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIesCLiWObLDFKKf3gJl0hll/ZT\nI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END EC PRIVATE KEY-----"), "keyId"), SecurityAlgorithms.EcdsaSha256), SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA).Build(),
            ClientBuilder.BuildTraditionalWebsiteClient("fapiWebite", "password", null, "http://localhost:5003/signin-oidc").SetIdTokenSignedResponseAlg(SecurityAlgorithms.EcdsaSha256).SetRequestObjectSigning(SecurityAlgorithms.EcdsaSha256).SetSigAuthorizationResponse(SecurityAlgorithms.EcdsaSha256).AddScope(Constants.StandardScopes.OpenIdScope, Constants.StandardScopes.Profile).UseClientTlsAuthentication("CN=sidClient, O=Internet Widgits Pty Ltd, S=BE, C=BE").AddSigningKey(new SigningCredentials(PemImporter.Import(new PemResult("-----BEGIN PUBLIC KEY-----\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIe\nsCLiWObLDFKKf3gJl0hll/ZTI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END PUBLIC KEY-----", "-----BEGIN EC PRIVATE KEY-----\nMHcCAQEEIDHtu+N0u38ZN7DF/TpycDfaUs8WfPGUB3UusR0uv3TVoAoGCCqGSM49\nAwEHoUQDQgAEK21CoKCA2Vk5zPM+7+vqtnrq4pIesCLiWObLDFKKf3gJl0hll/ZT\nI5ww/oRrKIXO/uRe9AkckkKwqrqqXGnvsQ==\n-----END EC PRIVATE KEY-----"), "keyId"), SecurityAlgorithms.EcdsaSha256), SecurityAlgorithms.EcdsaSha256, SecurityKeyTypes.ECDSA).Build()
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

        public static ICollection<IdentityProvisioningDefinition> IdProvisioningDefinitions = new List<IdentityProvisioningDefinition>
        {
            Scim
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
            Constants.StandardRealms.Master
        };

        public static ICollection<IdentityProvisioning> IdentityProvisiongLst => new List<IdentityProvisioning>
        {
            IdentityProvisioningBuilder.Create(Scim, "SCIM", "SCIM", new SyncSCIMRepresentationsOptions
            {
                Count = 1,
                SCIMEdp = "http://localhost:5002"
            }).Build()
        };
    }
}
