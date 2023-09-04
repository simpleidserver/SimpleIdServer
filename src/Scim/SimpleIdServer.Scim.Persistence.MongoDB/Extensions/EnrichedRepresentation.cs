// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public record EnrichedRepresentation
    {
        public SCIMRepresentationModel Representation { get; set; }
        public IEnumerable<SCIMRepresentationAttribute> FlatAttributes { get; set; }
    }
}
