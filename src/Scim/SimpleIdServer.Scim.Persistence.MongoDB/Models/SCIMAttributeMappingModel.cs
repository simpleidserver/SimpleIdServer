// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMAttributeMappingModel
    {
        public string Id { get; set; }
        public string SourceResourceType { get; set; }
        public string SourceAttributeSelector { get; set; }
        public string TargetResourceType { get; set; }
        public string TargetAttributeId { get; set; }
    }
}
