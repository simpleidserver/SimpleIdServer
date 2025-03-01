// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;
using static MassTransit.ValidationResultExtensions;

namespace SimpleIdServer.Scim.Helpers
{
    public class RepresentationSyncResult
    {
        public List<SCIMRepresentationAttribute> AddedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();
        public List<SCIMRepresentationAttribute> RemovedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();
        public List<SCIMRepresentationAttribute> UpdatedRepresentationAttributes { get; set; } = new List<SCIMRepresentationAttribute>();

        public void AddReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs) => AddedRepresentationAttributes.AddRange(attrs);

        public void RemoveReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs)
        {
            var ids = RemovedRepresentationAttributes.Select(a => a.Id);
            var filteredAttrs = attrs.Where(a => !ids.Contains(a.Id));
            RemovedRepresentationAttributes.AddRange(filteredAttrs);
        }

        public void UpdateReferenceAttributes(IEnumerable<SCIMRepresentationAttribute> attrs, IEnumerable<RepresentationSyncResult> result)
        {
            var filteredAttrs = attrs.Where(a => CanUpdate(a) && result.All(r => r.CanUpdate(a)));
            UpdatedRepresentationAttributes.AddRange(filteredAttrs);
        }

        public bool CanUpdate(SCIMRepresentationAttribute attr)
        {
            var allIds = AddedRepresentationAttributes.Select(a => a.Id).Union(RemovedRepresentationAttributes.Select(a => a.Id)).ToList();
            return !allIds.Contains(attr.Id);
        }
    }
}
