// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class ReplaceRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public ReplaceRepresentationCommand(string id, string resourceType, RepresentationParameter representation, string location)
        {
            Id = id;
            ResourceType = resourceType;
            Representation = representation;
            Location = location;
        }

        public string Id { get; private set; }
        public string ResourceType{ get; private set; }
        public RepresentationParameter Representation { get; private set; }
        public string Location { get; }
    }
}
