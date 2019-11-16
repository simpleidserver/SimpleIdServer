// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class DeleteRepresentationCommand : ISCIMCommand<bool>
    {
        public DeleteRepresentationCommand(string id, string resourceType)
        {
            Id = id;
            ResourceType = resourceType;
        }

        public string Id { get; set; }
        public string ResourceType { get; set; }
    }
}
