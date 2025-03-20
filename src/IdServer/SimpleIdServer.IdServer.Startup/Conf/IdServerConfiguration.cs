// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
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

namespace SimpleIdServer.IdServer.Startup.Conf;

public class IdServerConfiguration
{
    public static ICollection<Scope> Scopes => new List<Scope>
    {
        SimpleIdServer.IdServer.Config.DefaultScopes.OpenIdScope,
        SimpleIdServer.IdServer.Config.DefaultScopes.Profile,
        SimpleIdServer.IdServer.Config.DefaultScopes.SAMLProfile,
        SimpleIdServer.IdServer.Config.DefaultScopes.GrantManagementQuery,
        SimpleIdServer.IdServer.Config.DefaultScopes.GrantManagementRevoke,
        SimpleIdServer.IdServer.Config.DefaultScopes.Users,
        SimpleIdServer.IdServer.Config.DefaultScopes.Register,
        SimpleIdServer.IdServer.Config.DefaultScopes.Provisioning,
        SimpleIdServer.IdServer.Config.DefaultScopes.Address,
        SimpleIdServer.IdServer.Config.DefaultScopes.Role,
        SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationSchemeProviders,
        SimpleIdServer.IdServer.Config.DefaultScopes.RegistrationWorkflows,
        SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationMethods,
        SimpleIdServer.Configuration.Constants.ConfigurationsScope,
        SimpleIdServer.IdServer.Config.DefaultScopes.ApiResources,
        SimpleIdServer.IdServer.Config.DefaultScopes.Auditing,
        SimpleIdServer.IdServer.Config.DefaultScopes.Scopes,
        SimpleIdServer.IdServer.Config.DefaultScopes.CertificateAuthorities,
        SimpleIdServer.IdServer.Config.DefaultScopes.Clients,
        SimpleIdServer.IdServer.Config.DefaultScopes.Realms,
        SimpleIdServer.IdServer.Config.DefaultScopes.Groups,
        SimpleIdServer.IdServer.Config.DefaultScopes.OfflineAccessScope,
        SimpleIdServer.IdServer.Config.DefaultScopes.CredentialConfigurations,
        SimpleIdServer.IdServer.Config.DefaultScopes.CredentialInstances,
        SimpleIdServer.IdServer.Config.DefaultScopes.DeferredCreds,
        SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities,
        SimpleIdServer.IdServer.Config.DefaultScopes.WebsiteAdministratorRole,
        ScimScope,
        IdProviderAdministratorScope,
        AppProviderAdministratorScope,
        SimpleIdServer.IdServer.Config.DefaultScopes.Acrs,
        SimpleIdServer.IdServer.Config.DefaultScopes.Workflows,
        SimpleIdServer.IdServer.Config.DefaultScopes.Forms,
        SimpleIdServer.IdServer.Config.DefaultScopes.RecurringJobs
    };

    public static ICollection<Client> Clients => new List<Client>
    {
        ClientBuilder.BuildWalletClient("walletClient", "password")
            .SetClientName("Wallet")
            .Build(),
        ClientBuilder.BuildTraditionalWebsiteClient("SIDS-manager", "password", null, "https://localhost:5002/*", "https://website.simpleidserver.com/*", "https://website.localhost.com/*", "http://website.localhost.com/*", "https://website.sid.svc.cluster.local/*")
            .EnableClientGrantType()
            .SetRequestObjectEncryption()
            .AddPostLogoutUri("https://localhost:5002/signout-callback-oidc").AddPostLogoutUri("https://website.sid.svc.cluster.local/signout-callback-oidc").AddPostLogoutUri("https://website.simpleidserver.com/signout-callback-oidc")
            .AddAuthDataTypes("photo")
            .SetClientName("SimpleIdServer manager")
            .SetBackChannelLogoutUrl("https://localhost:5002/bc-logout")
            .SetClientLogoUri("https://cdn.logo.com/hotlink-ok/logo-social.png")
            .AddScope(
                SimpleIdServer.IdServer.Config.DefaultScopes.Role,
                SimpleIdServer.IdServer.Config.DefaultScopes.OpenIdScope, 
                SimpleIdServer.IdServer.Config.DefaultScopes.Profile, 
                SimpleIdServer.IdServer.Config.DefaultScopes.Provisioning,
                SimpleIdServer.IdServer.Config.DefaultScopes.Users,
                SimpleIdServer.IdServer.Config.DefaultScopes.Workflows,
                SimpleIdServer.IdServer.Config.DefaultScopes.Acrs,
                SimpleIdServer.Configuration.Constants.ConfigurationsScope, 
                SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationSchemeProviders, 
                SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationMethods, 
                SimpleIdServer.IdServer.Config.DefaultScopes.RegistrationWorkflows,
                SimpleIdServer.IdServer.Config.DefaultScopes.ApiResources,
                SimpleIdServer.IdServer.Config.DefaultScopes.Auditing,
                SimpleIdServer.IdServer.Config.DefaultScopes.Scopes, 
                SimpleIdServer.IdServer.Config.DefaultScopes.CertificateAuthorities,
                SimpleIdServer.IdServer.Config.DefaultScopes.Clients,
                SimpleIdServer.IdServer.Config.DefaultScopes.Realms, 
                SimpleIdServer.IdServer.Config.DefaultScopes.Groups,
                SimpleIdServer.IdServer.Config.DefaultScopes.WebsiteAdministratorRole,
                SimpleIdServer.IdServer.Config.DefaultScopes.Forms,
                SimpleIdServer.IdServer.Config.DefaultScopes.RecurringJobs,
                SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities).Build(),
        ClientBuilder.BuildTraditionalWebsiteClient("swaggerClient", "password", null, "https://localhost:5001/swagger/oauth2-redirect.html", "https://localhost:5001/(.*)/swagger/oauth2-redirect.html", "http://localhost").AddScope(
            SimpleIdServer.IdServer.Config.DefaultScopes.Provisioning, 
            SimpleIdServer.IdServer.Config.DefaultScopes.Users, 
            SimpleIdServer.IdServer.Config.DefaultScopes.Acrs, 
            SimpleIdServer.Configuration.Constants.ConfigurationsScope,
            SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationSchemeProviders,
            SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationMethods, 
            SimpleIdServer.IdServer.Config.DefaultScopes.RegistrationWorkflows, 
            SimpleIdServer.IdServer.Config.DefaultScopes.ApiResources, 
            SimpleIdServer.IdServer.Config.DefaultScopes.Auditing, 
            SimpleIdServer.IdServer.Config.DefaultScopes.Scopes, 
            SimpleIdServer.IdServer.Config.DefaultScopes.CertificateAuthorities,
            SimpleIdServer.IdServer.Config.DefaultScopes.Clients, 
            SimpleIdServer.IdServer.Config.DefaultScopes.Realms,
            SimpleIdServer.IdServer.Config.DefaultScopes.Groups,
            SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities).Build(),
        ClientBuilder.BuildTraditionalWebsiteClient("postman", "password", null, "http://localhost").EnableClientGrantType().AddScope(
            SimpleIdServer.IdServer.Config.DefaultScopes.Provisioning,
            SimpleIdServer.IdServer.Config.DefaultScopes.Users,
            SimpleIdServer.IdServer.Config.DefaultScopes.Acrs,
            SimpleIdServer.Configuration.Constants.ConfigurationsScope,
            SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationSchemeProviders,
            SimpleIdServer.IdServer.Config.DefaultScopes.AuthenticationMethods,
            SimpleIdServer.IdServer.Config.DefaultScopes.RegistrationWorkflows,
            SimpleIdServer.IdServer.Config.DefaultScopes.ApiResources,
            SimpleIdServer.IdServer.Config.DefaultScopes.Auditing,
            SimpleIdServer.IdServer.Config.DefaultScopes.Scopes,
            SimpleIdServer.IdServer.Config.DefaultScopes.CertificateAuthorities,
            SimpleIdServer.IdServer.Config.DefaultScopes.Clients,
            SimpleIdServer.IdServer.Config.DefaultScopes.Realms,
            SimpleIdServer.IdServer.Config.DefaultScopes.Groups,
            SimpleIdServer.IdServer.Federation.IdServerFederationConstants.StandardScopes.FederationEntities).Build()
    };

    public static List<GotifySession> Sessions = new List<GotifySession>
    {
        new GotifySession { ApplicationToken = "AvSdAw5ILVOdc7g", ClientToken = "CY2St_LANPO5L7P" },
        new GotifySession { ApplicationToken = "ADIeCkMigAnGLmq", ClientToken = "C9M4RGtX.OlYD1q" }
    };

    // TODO : Move to a configuration file !!
    public static List<PresentationDefinition> PresentationDefinitions = new List<PresentationDefinition>
    {
        PresentationDefinitionBuilder.New("universitydegree_vp", "University Degree").AddLdpVcInputDescriptor("UniversityDegree", "UniversityDegree", "UniversityDegree").Build()
    };

    // TODO : Move to a configuration file !!
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
