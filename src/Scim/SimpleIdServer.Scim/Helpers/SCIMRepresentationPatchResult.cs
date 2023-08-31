// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class SCIMRepresentationPatchResult
    {
        public List<SCIMPatchResult> Patches { get; private set; } = new List<SCIMPatchResult>();

        public void Remove(SCIMRepresentationAttribute attr)
        {
            Patches = Patches.Where(p => !(p.Operation == SCIMPatchOperations.REPLACE && p.Attr?.Id == attr.Id)).ToList();
            Patches.Add(new SCIMPatchResult { Attr = attr, Path = attr.FullPath, Operation = SCIMPatchOperations.REMOVE });
        }

        public void Add(SCIMRepresentationAttribute attr) => Patches.Add(new SCIMPatchResult { Attr = attr, Operation = SCIMPatchOperations.ADD, Path = attr.FullPath });

        public void Update(SCIMRepresentationAttribute attr) => Patches.Add(new SCIMPatchResult { Attr = attr, Operation = SCIMPatchOperations.REPLACE, Path = attr.FullPath });

        public void AddExternalId() => Patches.Add(new SCIMPatchResult { Attr = null, Operation = SCIMPatchOperations.REPLACE, Path = StandardSCIMRepresentationAttributes.ExternalId });
    }
}
