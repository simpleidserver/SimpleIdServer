// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Commands
{
    public class ReplaceRepresentationCommand : ISCIMCommand<ReplaceRepresentationResult>
    {
        public ReplaceRepresentationCommand(string id, string resourceType, RepresentationParameter representation, string location, string realm)
        {
            Id = id;
            ResourceType = resourceType;
            Representation = representation;
            Location = location;
            Realm = realm;
        }

        public string Id { get; private set; }
        public string ResourceType{ get; private set; }
        public RepresentationParameter Representation { get; private set; }
        public string Location { get; }
        public string Realm { get; set; }
    }

    public class ReplaceRepresentationResult
    {
        public bool IsReplaced { get; private set; }
        public SCIMRepresentation Representation {  get; private set; }
        public List<SCIMPatchResult> PatchOperations { get; private set; } = new List<SCIMPatchResult>();

        public ReplaceRepresentationResult()
        {
        }

        public ReplaceRepresentationResult(SCIMRepresentation representation, List<SCIMPatchResult> patchOperations)
        {
            Representation = representation;
            PatchOperations = patchOperations;
        }

        public static ReplaceRepresentationResult NoReplacement()
        {
            return new ReplaceRepresentationResult { IsReplaced = false };
        }

        public static ReplaceRepresentationResult Ok(SCIMRepresentation representation, List<SCIMPatchResult> patchOperations)
        {
            return new ReplaceRepresentationResult { IsReplaced = true, Representation = representation, PatchOperations = patchOperations };
        }
    }
}
