// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class AddRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public AddRepresentationCommand(string resourceType, JObject representation)
        {
            ResourceType = resourceType;
            Representation = representation;
        }

        public string ResourceType { get; set; }
        public JObject Representation { get; }
    }
}
