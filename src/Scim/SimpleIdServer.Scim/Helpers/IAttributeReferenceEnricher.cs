// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public interface IAttributeReferenceEnricher
    {
        Task Enrich(string resourceType, IEnumerable<SCIMRepresentation> representationLst, string baseUrl);
    }
}
