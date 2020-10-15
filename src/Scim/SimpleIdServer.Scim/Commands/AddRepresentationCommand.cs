// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class AddRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public AddRepresentationCommand(string resourceType, RepresentationParameter representation)
        {
            ResourceType = resourceType;
            Representation = representation;
        }

        public string ResourceType { get; set; }
        public RepresentationParameter Representation { get; }
    }
}
