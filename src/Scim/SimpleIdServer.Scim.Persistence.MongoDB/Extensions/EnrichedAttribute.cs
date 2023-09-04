// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public record EnrichedAttribute
    {
        public SCIMRepresentationAttribute Attribute { get; set; }
        public IEnumerable<EnrichedAttribute> Children { get; set; }
    }
}
