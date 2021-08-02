// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Models
{
    public class SCIMRepresentationAttributeModel : SCIMRepresentationAttribute
    {
        public SCIMRepresentationAttributeModel()
        {

        }

        public SCIMRepresentationAttributeModel(SCIMRepresentationAttribute attr, string representationId, string representationResourceType)
        {
            Id = attr.Id;
            AttributeId = attr.AttributeId;
            ParentAttributeId = attr.ParentAttributeId;
            SchemaAttributeId = attr.SchemaAttributeId;
            FullPath = attr.FullPath;
            ValueString = attr.ValueString;
            ValueBoolean = attr.ValueBoolean;
            ValueInteger = attr.ValueInteger;
            ValueDateTime = attr.ValueDateTime;
            ValueReference = attr.ValueReference;
            ValueDecimal = attr.ValueDecimal;
            RepresentationId = representationId;
            RepresentationResourceType = representationResourceType;
        }

        public string RepresentationResourceType { get; set; }
    }
}
