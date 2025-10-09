// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Infrastructure;

namespace SimpleIdServer.Scim.Commands
{
    public class DeleteRepresentationCommand : ISCIMCommand<SCIMRepresentation>
    {
        public DeleteRepresentationCommand(string id, string resourceType, string location, string realm, bool isPublishEvtsEnabled)
        {
            Id = id;
            ResourceType = resourceType;
            Location = location;
            Realm = realm;
            IsPublishEvtsEnabled = isPublishEvtsEnabled;
        }

        public string Id { get; set; }
        public string ResourceType { get; set; }
        public string Location { get; }
        public string Realm { get; set; }
        public bool IsPublishEvtsEnabled { get; set; }
    }
}
