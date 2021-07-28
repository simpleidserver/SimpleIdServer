// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class PatchRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public PatchRepresentationCommand(string id, string resourceType, PatchRepresentationParameter patchRepresentation)
        {
            Id = id;
            ResourceType = resourceType;
            PatchRepresentation = patchRepresentation;
        }

        public string Id { get; private set; }
        public PatchRepresentationParameter PatchRepresentation { get; private set; }
        public string ResourceType { get; private set; }
    }
}
