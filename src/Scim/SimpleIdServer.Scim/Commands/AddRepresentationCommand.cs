// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class AddRepresentationCommand : ISCIMCommand<string>
    {
        public AddRepresentationCommand(string resourceType, RepresentationParameter representation, string location)
        {
            ResourceType = resourceType;
            Representation = representation;
            Location = location;
        }

        public string ResourceType { get; set; }
        public RepresentationParameter Representation { get; }
        public string Location { get; }
    }
}
