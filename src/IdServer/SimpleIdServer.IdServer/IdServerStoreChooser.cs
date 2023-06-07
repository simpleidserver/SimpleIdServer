// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.IdServer
{
    public class IdServerStoreChooser
    {
        private readonly IServiceCollection _services;
        private readonly AuthenticationBuilder _authBuilder;

        public IdServerStoreChooser(IServiceCollection services, AuthenticationBuilder authBuilder) 
        {
            _services = services;
            _authBuilder = authBuilder;
        }

        public IdServerBuilder UseEFStore(Action<DbContextOptionsBuilder> dbCallback)
        {
            _services.AddStore(dbCallback);
            return new IdServerBuilder(_services, _authBuilder);
        }

        public IdServerBuilder UseInMemoryStore(Action<IdServerInMemoryStoreBuilder> callback)
        {
            _services.AddStore();
            var serviceProvider = _services.BuildServiceProvider();
            callback(new IdServerInMemoryStoreBuilder(serviceProvider));
            return new IdServerBuilder(_services, _authBuilder);
        }
    }

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
            if(!storeDbContext.UmaResources.Any())
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

        public IdServerInMemoryStoreBuilder AddInMemoryCredentialOffers(ICollection<UserCredentialOffer> offers)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if(!storeDbContext.CredentialOffers.Any())
            {
                storeDbContext.CredentialOffers.AddRange(offers);
            }

            return this;
        }

        public IdServerInMemoryStoreBuilder AddInMemoryCredentialTemplates(ICollection<CredentialTemplate> credentialTemplates)
        {
            var storeDbContext = _serviceProvider.GetService<StoreDbContext>();
            if(!storeDbContext.CredentialTemplates.Any())
            {
                storeDbContext.CredentialTemplates.AddRange(credentialTemplates);
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
}
