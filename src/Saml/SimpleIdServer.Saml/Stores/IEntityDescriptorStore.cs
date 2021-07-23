// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Xsd;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Stores
{
    public interface IEntityDescriptorStore
    {
        Task<EntityDescriptorType> Get(string metadataUrl, CancellationToken cancellationToken);
    }
}
