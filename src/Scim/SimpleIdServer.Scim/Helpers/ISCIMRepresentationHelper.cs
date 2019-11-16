// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Helpers
{
    public interface ISCIMRepresentationHelper
    {
        SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, ICollection<SCIMSchema> schemas);
    }
}