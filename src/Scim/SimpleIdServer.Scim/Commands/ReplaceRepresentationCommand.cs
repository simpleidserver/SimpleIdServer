// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class ReplaceRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public ReplaceRepresentationCommand(string id, string resourceType, JObject representation)
        {
            Id = id;
            ResourceType = resourceType;
            Representation = representation;
        }

        public string Id { get; private set; }
        public string ResourceType{ get; private set; }
        public JObject Representation { get; private set; }
    }
}
