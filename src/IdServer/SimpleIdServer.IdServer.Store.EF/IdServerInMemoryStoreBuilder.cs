﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation.Domains;

namespace SimpleIdServer.IdServer.Store.EF;

public class IdServerInMemoryStoreBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public IdServerInMemoryStoreBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        AddInMemoryAcr(new List<AuthenticationContextClassReference> { Constants.StandardAcrs.FirstLevelAssurance, Constants.StandardAcrs.IapSilver });
    }

    public IdServerInMemoryStoreBuilder AddInMemoryRealms(ICollection<Domains.Realm> realms)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.Realms.Any())
        {
            storeDbContext.Realms.AddRange(realms);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryScopes(ICollection<Domains.Scope> scopes)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.Scopes.Any())
        {
            storeDbContext.Scopes.AddRange(scopes);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryBCAuthorize(ICollection<BCAuthorize> bcAuthorizeLst)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.BCAuthorizeLst.Any())
        {
            storeDbContext.BCAuthorizeLst.AddRange(bcAuthorizeLst);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryClients(ICollection<Client> clients)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.Clients.Any())
        {
            storeDbContext.Clients.AddRange(clients);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryApiResources(ICollection<ApiResource> apiResources)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.ApiResources.Any())
        {
            storeDbContext.ApiResources.AddRange(apiResources);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryAcr(ICollection<AuthenticationContextClassReference> acrs)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.Acrs.Any())
        {
            storeDbContext.Acrs.AddRange(acrs);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemorySerializedFileKeys(ICollection<SerializedFileKey> keys)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.SerializedFileKeys.Any())
        {
            storeDbContext.SerializedFileKeys.AddRange(keys);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryCAs(ICollection<CertificateAuthority> cas)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.CertificateAuthorities.Any())
        {
            storeDbContext.CertificateAuthorities.AddRange(cas);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryUsers(ICollection<User> users)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.Users.Any())
        {
            storeDbContext.Users.AddRange(users);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryAuthenticationSchemeProviderDefinitions(ICollection<Domains.AuthenticationSchemeProviderDefinition> definitions)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.AuthenticationSchemeProviderDefinitions.Any())
        {
            storeDbContext.AuthenticationSchemeProviderDefinitions.AddRange(definitions);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryAuthenticationSchemeProviders(ICollection<Domains.AuthenticationSchemeProvider> providers)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.AuthenticationSchemeProviders.Any())
        {
            storeDbContext.AuthenticationSchemeProviders.AddRange(providers);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryUMAResources(ICollection<UMAResource> umaResources)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.UmaResources.Any())
        {
            storeDbContext.UmaResources.AddRange(umaResources);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryGroups(ICollection<Group> groups)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.Groups.Any())
        {
            storeDbContext.Groups.AddRange(groups);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryUserSessions(ICollection<UserSession> sessions)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.UserSession.Any())
        {
            storeDbContext.UserSession.AddRange(sessions);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryDeviceCodes(ICollection<DeviceAuthCode> deviceCodes)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.DeviceAuthCodes.Any())
        {
            storeDbContext.DeviceAuthCodes.AddRange(deviceCodes);
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryFederatonEntities(ICollection<FederationEntity> federationEntities)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.FederationEntities.Any())
        {
            storeDbContext.FederationEntities.AddRange(federationEntities);
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryKeys(Domains.Realm realm, ICollection<SigningCredentials> signingCredentials, ICollection<EncryptingCredentials> encryptingCredentials)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.SerializedFileKeys.Any())
        {
            storeDbContext.SerializedFileKeys.AddRange(signingCredentials.Select(c => InMemoryKeyStore.Convert(c, realm)).ToList());
            storeDbContext.SerializedFileKeys.AddRange(encryptingCredentials.Select(c => InMemoryKeyStore.Convert(c, realm)).ToList());
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryIdentityProvisioning(ICollection<IdentityProvisioning> identityProvisioningLst)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.IdentityProvisioningLst.Any())
        {
            storeDbContext.IdentityProvisioningLst.AddRange(identityProvisioningLst);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryIdentityProvisioningDefinitions(ICollection<IdentityProvisioningDefinition> identityProvisioningDefinitions)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.IdentityProvisioningDefinitions.Any())
        {
            storeDbContext.IdentityProvisioningDefinitions.AddRange(identityProvisioningDefinitions);
            storeDbContext.SaveChanges();
        }

        return this;
    }

    public IdServerInMemoryStoreBuilder AddInMemoryUMAPendingRequests(ICollection<UMAPendingRequest> umaPendingRequests)
    {
        var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
        if (!storeDbContext.UmaPendingRequest.Any())
        {
            storeDbContext.UmaPendingRequest.AddRange(umaPendingRequests);
            storeDbContext.SaveChanges();
        }

        return this;
    }
}
