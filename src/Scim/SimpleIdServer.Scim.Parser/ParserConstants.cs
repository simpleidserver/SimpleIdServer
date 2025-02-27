// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Parser
{
    public static class ParserConstants
    {

        public static Dictionary<string, string> MappingStandardAttributePathToProperty = new Dictionary<string, string>
        {
            { StandardSCIMRepresentationAttributes.Id, "Id" },
            { StandardSCIMRepresentationAttributes.ExternalId, "ExternalId" },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}", "ResourceType" },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}", "Created" },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}", "LastModified" },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}", "Version" },
        };

        public static Dictionary<string, SCIMSchemaAttributeTypes> MappingStandardAttributeTypeToType = new Dictionary<string, SCIMSchemaAttributeTypes>
        {
            { StandardSCIMRepresentationAttributes.Id, SCIMSchemaAttributeTypes.STRING},
            { StandardSCIMRepresentationAttributes.ExternalId, SCIMSchemaAttributeTypes.STRING},
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}", SCIMSchemaAttributeTypes.STRING },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}", SCIMSchemaAttributeTypes.DATETIME },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}", SCIMSchemaAttributeTypes.DATETIME },
            { $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}", SCIMSchemaAttributeTypes.STRING }
        };
    }
}
