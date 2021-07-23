// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Stores
{
    public class InMemoryEntityDescriptorStore : IEntityDescriptorStore
    {
        private Dictionary<string, EntityDescriptorType> _entityDescriptors;

        public InMemoryEntityDescriptorStore()
        {
            _entityDescriptors = new Dictionary<string, EntityDescriptorType>();
        }

        public async Task<EntityDescriptorType> Get(string metadataUrl, CancellationToken cancellationToken)
        {
            if (_entityDescriptors.ContainsKey(metadataUrl))
            {
                return _entityDescriptors[metadataUrl];
            }

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetAsync(metadataUrl, cancellationToken);
                httpResponse.EnsureSuccessStatusCode();
                var str = await httpResponse.Content.ReadAsStringAsync();
                var entityDescriptor = str.DeserializeXml<EntityDescriptorType>();
                _entityDescriptors.Add(metadataUrl, entityDescriptor);
                return entityDescriptor;
            }
        }
    }
}
