// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultScopes
{
    public static List<string> DefaultRealmScopeNames => new List<string>
    {
        OpenIdScope.Name,
        Profile.Name,
        SAMLProfile.Name,
        Role.Name,
        WebsiteAdministratorRole.Name
    };

    public static List<Scope> All => new List<Scope>
    {
        Profile,
        ConfigurationsScope,
        Email,
        Address,
        Phone,
        Role,
        OpenIdScope,
        OfflineAccessScope,
        ScimScope,
        GrantManagementQuery,
        GrantManagementRevoke,
        UmaProtection,
        Users,
        AuthenticationSchemeProviders,
        AuthenticationMethods,
        RegistrationWorkflows,
        Register,
        SAMLProfile,
        Provisioning,
        CredentialConfigurations,
        CredentialInstances,
        DeferredCreds,
        Acrs,
        Workflows,
        ApiResources,
        Auditing,
        Scopes,
        CertificateAuthorities,
        RecurringJobs,
        Clients,
        Forms,
        Realms,
        Groups,
        Templates
    };

    public static List<Scope> AdministrativeScopes
    {
        get
        {
            var scopes = new List<Scope> { WebsiteAdministratorRole };
            scopes.AddRange(RealmRoleBuilder.BuildAdministrativeRole(DefaultRealms.Master));
            return scopes;
        }
    }

    public static List<Scope> AdministrativeRoScopes
    {
        get
        {
            return RealmRoleBuilder.BuildAdministrativeRoleRo(DefaultRealms.Master);
        }
    }

    public static Scope Profile = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "profile",
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.Name,
            Config.DefaultClaimMappers.FamilyName,
            Config.DefaultClaimMappers.UniqueName,
            Config.DefaultClaimMappers.GivenName,
            Config.DefaultClaimMappers.MiddleName,
            Config.DefaultClaimMappers.NickName,
            Config.DefaultClaimMappers.PreferredUserName,
            Config.DefaultClaimMappers.Profile,
            Config.DefaultClaimMappers.Picture,
            Config.DefaultClaimMappers.WebSite,
            Config.DefaultClaimMappers.Gender,
            Config.DefaultClaimMappers.BirthDate,
            Config.DefaultClaimMappers.ZoneInfo,
            Config.DefaultClaimMappers.Locale,
            Config.DefaultClaimMappers.UpdatedAt
        },
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.IDENTITY,
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope ConfigurationsScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "configurations",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.APIRESOURCE,
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Email = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "email",
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.Email,
            Config.DefaultClaimMappers.EmailVerified
        },
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.IDENTITY,
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Address = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "address",
        ClaimMappers = Config.DefaultClaimMappers.Address,
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.IDENTITY,
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Phone = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "phone",
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.PhoneNumber,
            Config.DefaultClaimMappers.PhoneNumberVerified
        },
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.IDENTITY,
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Role = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "role",
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.Role
        },
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.IDENTITY,
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope OpenIdScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "openid",
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.Subject
        },
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Type = ScopeTypes.IDENTITY,
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope OfflineAccessScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Protocol = ScopeProtocols.OAUTH,
        Name = "offline_access",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope ScimScope = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.IDENTITY,
        Name = "scim",
        IsExposedInConfigurationEdp = true,
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.ScimId
        },
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OPENID,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope GrantManagementQuery = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.IDENTITY,
        Name = "grant_management_query",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = false,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope GrantManagementRevoke = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.IDENTITY,
        Name = "grant_management_revoke",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = false,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope UmaProtection = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.IDENTITY,
        Name = "uma_protection",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OPENID,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Users = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "users",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope AuthenticationSchemeProviders = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "authenticationschemeproviders",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope AuthenticationMethods = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "authenticationmethods",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope RegistrationWorkflows = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "registrationworkflows",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Register = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "register",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope SAMLProfile = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.IDENTITY,
        Name = "saml_profile",
        Realms = new List<Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.SAML,
        IsExposedInConfigurationEdp = false,
        ClaimMappers = new List<ScopeClaimMapper>
        {
            Config.DefaultClaimMappers.SAMLNameIdentifier,
            Config.DefaultClaimMappers.SAMLName
        },
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Provisioning = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "provisioning",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope CredentialConfigurations = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "credconfs",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope CredentialInstances = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "credinstances",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope DeferredCreds = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "deferredcreds",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Acrs = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "acrs",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Workflows = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "workflows",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope ApiResources = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "apiresources",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Auditing = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "auditing",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Scopes = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "scopes",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope CertificateAuthorities = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "certificateauthorities",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope RecurringJobs = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "recurringjobs",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Clients = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "clients",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Migrations = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "migrations",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Forms = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "forms",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Realms = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "realms",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope Groups = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "groups",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
    public static Scope WebsiteAdministratorRole = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Name = "SIDS-manager/administrator",
        Type = ScopeTypes.ROLE,
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        Description = "Administrator",
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow,
    };
    public static Scope Templates = new Scope
    {
        Id = Guid.NewGuid().ToString(),
        Type = ScopeTypes.APIRESOURCE,
        Name = "templates",
        Realms = new List<Domains.Realm>
        {
            DefaultRealms.Master
        },
        Protocol = ScopeProtocols.OAUTH,
        IsExposedInConfigurationEdp = true,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow
    };
}
