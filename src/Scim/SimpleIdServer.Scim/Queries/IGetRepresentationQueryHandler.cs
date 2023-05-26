// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Queries
{
    public interface IGetRepresentationQueryHandler
    {
        Task<SCIMRepresentation> Handle(string id, GetSCIMResourceRequest parameter, string resourceType);
    }
}
