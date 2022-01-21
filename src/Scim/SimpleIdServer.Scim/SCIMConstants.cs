// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim
{
    public static class SCIMConstants
    {
        public const string AuthenticationScheme = "SimpleIdServerSCIM";
        public const string STANDARD_SCIM_CONTENT_TYPE = "application/scim+json";

        public static Dictionary<string, string> MappingScimResourceTypeToCommonType = new Dictionary<string, string>
        {
            { SCIMEndpoints.User, CommonConstants.ResourceTypes.ScimUser },
            { SCIMEndpoints.Group, CommonConstants.ResourceTypes.ScimGroup },
        };

        public static class ErrorSCIMTypes
        {
            public const string InvalidSyntax = "invalidSyntax";
            public const string Uniqueness = "uniqueness";
            public const string InternalServerError = "internalServerError";
            public const string Unknown = "unknown";
            public const string Mutability = "mutability";
            public const string InvalidFilter = "invalidFilter";
            public const string SchemaViolated = "schemaViolated";
            public const string TooLarge = "tooLarge";
            public const string NoTarget = "noTarget";
        }

        public static ICollection<string> StandardSCIMCommonRepresentationAttributes = new List<string>
        {
            StandardSCIMRepresentationAttributes.Id,
            StandardSCIMRepresentationAttributes.ExternalId,
            StandardSCIMRepresentationAttributes.Meta
        };

        public static class PathOperationAttributes
        {
            public const string Operation = "op";
            public const string Path = "path";
            public const string Value = "value";
            public const string Operations = "Operations";
        }

        public static class StandardSCIMMetaAttributes
        {
            public const string ResourceType = "resourceType";
            public const string Created = "created";
            public const string LastModified = "lastModified";
            public const string Location = "location";
            public const string Version = "version";
        }

        public static class StandardSCIMSearchAttributes
        {
            public const string Attributes = "attributes";
            public const string ExcludedAttributes = "excludedAttributes";
            public const string Filter = "filter";
            public const string SortBy = "sortBy";
            public const string SortOrder = "sortOrder";
            public const string StartIndex = "startIndex";
            public const string Count = "count";
        }

        public static List<SCIMAttributeMapping> StandardAttributeMapping = new List<SCIMAttributeMapping>
        {
            new SCIMAttributeMapping
            {
                Id = Guid.NewGuid().ToString(),
                SourceAttributeId = StandardSchemas.UserSchema.Attributes.First(a => a.Name == "groups").Id,
                SourceResourceType = StandardSchemas.UserSchema.ResourceType,
                SourceAttributeSelector = "groups",
                TargetResourceType = StandardSchemas.GroupSchema.ResourceType,
                TargetAttributeId = StandardSchemas.GroupSchema.Attributes.First(a => a.Name == "members").Id
            }
        };

        public static List<string> AllStandardPath = new List<string>
        {
             StandardSCIMRepresentationAttributes.Id,
             StandardSCIMRepresentationAttributes.ExternalId,
             StandardSCIMRepresentationAttributes.Meta,
             $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}",
             $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}",
             $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}",
             $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}"
        };

        public static Dictionary<SCIMSchemaAttributeTypes, Type> MappingSchemaAttrTypeToType = new Dictionary<SCIMSchemaAttributeTypes, Type>
        {
            { SCIMSchemaAttributeTypes.BOOLEAN, typeof(bool) },
            { SCIMSchemaAttributeTypes.STRING, typeof(string) },
            { SCIMSchemaAttributeTypes.INTEGER, typeof(int) },
            { SCIMSchemaAttributeTypes.DATETIME, typeof(DateTime) },
            { SCIMSchemaAttributeTypes.DECIMAL, typeof(decimal) }
        };
    }
}