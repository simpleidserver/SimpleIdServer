// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class DeleteRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public DeleteRepresentationCommand(string id, string resourceType, string location)
        {
            Id = id;
            ResourceType = resourceType;
            Location = location;
        }

        public string Id { get; set; }
        public string ResourceType { get; set; }
        public string Location { get; }
    }
}
