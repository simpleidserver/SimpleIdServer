// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationSyncResult
    {
        public List<SCIMRepresentationAttribute> AddedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();
        public List<SCIMRepresentationAttribute> RemovedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();
        public List<SCIMRepresentationAttribute> UpdatedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();

        public void AddReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs) => AddedRepresentationAttributes.AddRange(attrs);

        public void RemoveReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs) => RemovedRepresentationAttributes.AddRange(attrs);

        public void UpdateReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs) => UpdatedRepresentationAttributes.AddRange(attrs);
    }
}
