// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Queries
{
    public interface IGetRepresentationQueryHandler
    {
        Task<GenericResult<SCIMRepresentation>> Handle(string realm, string id, GetSCIMResourceRequest parameter, string resourceType, CancellationToken cancellationToken);
    }
}
