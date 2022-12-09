// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.Scim.Domains
{
    public class SCIMRepresentationIndirectReference : ICloneable
    {
        public int NbReferences { get; set; }
        public string TargetReferenceId { get; set; }
        public string TargetAttributeId { get; set; }

        public object Clone()
        {
            return new SCIMRepresentationIndirectReference
            {
                NbReferences = NbReferences,
                TargetReferenceId = TargetReferenceId,
                TargetAttributeId = TargetAttributeId
            };
        }
    }
}
