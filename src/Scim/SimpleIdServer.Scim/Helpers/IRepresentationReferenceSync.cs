// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Helpers
{
    public interface IRepresentationReferenceSync
    {
        Task<RepresentationSyncResult> Sync(IEnumerable<SCIMAttributeMapping> attributeMappingLst, string resourceType, SCIMRepresentation oldScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, bool updateAllReferences = false, bool isScimRepresentationRemoved = false);
        Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation newSourceScimRepresentation, ICollection<SCIMPatchResult> patchOperations, string location);
        Task<RepresentationSyncResult> Sync(string resourceType, SCIMRepresentation oldSourceScimRepresentation, SCIMRepresentation newSourceScimRepresentation, string location, bool updateAllReferences = true, bool isScimRepresentationRemoved = false);
        Task<bool> IsReferenceProperty(ICollection<string> attributes);
    }
}
