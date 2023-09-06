// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class ReplaceRepresentationCommand : ISCIMCommand<ReplaceRepresentationResult>
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

    public class ReplaceRepresentationResult
    {
        public bool IsReplaced { get; private set; }
        public SCIMRepresentation Representation {  get; private set; }

        public ReplaceRepresentationResult()
        {

        }

        public ReplaceRepresentationResult(SCIMRepresentation representation)
        {
            Representation = representation;
        }

        public static ReplaceRepresentationResult NoReplacement()
        {
            return new ReplaceRepresentationResult { IsReplaced = false };
        }

        public static ReplaceRepresentationResult Ok(SCIMRepresentation representation)
        {
            return new ReplaceRepresentationResult { IsReplaced = true, Representation = representation };
        }
    }
}
