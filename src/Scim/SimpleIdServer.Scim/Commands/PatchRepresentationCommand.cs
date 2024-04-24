// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class PatchRepresentationCommand : ISCIMCommand<PatchRepresentationResult>
    {
        public PatchRepresentationCommand(string id, string resourceType, PatchRepresentationParameter patchRepresentation, string location, string realm)
        {
            Id = id;
            ResourceType = resourceType;
            PatchRepresentation = patchRepresentation;
            Location = location;
            Realm = realm;
        }

        public string Id { get; private set; }
        public PatchRepresentationParameter PatchRepresentation { get; private set; }
        public string ResourceType { get; private set; }
        public string Location { get; }
        public string Realm { get; set; }
    }

    public class PatchRepresentationResult
    {
        public bool IsPatched { get; private set; }
        public SCIMRepresentation Representation { get; private set; }

        public static PatchRepresentationResult NoPatch()
        {
            return new PatchRepresentationResult { IsPatched = false };
        }

        public static PatchRepresentationResult Ok(SCIMRepresentation representation)
        {
            return new PatchRepresentationResult { IsPatched = true, Representation = representation };
        }
    }
}
