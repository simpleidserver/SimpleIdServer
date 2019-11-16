// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMSchemaExtensions
    {
        public static JObject ToResponse(this SCIMSchema schema)
        {
            var jObj = new JObject
            {
                { SCIMConstants.StandardSCIMRepresentationAttributes.Id, schema.Id },
                { SCIMConstants.StandardSCIMRepresentationAttributes.Name,  schema.Name},
                { SCIMConstants.StandardSCIMRepresentationAttributes.Description,  schema.Name}
            };

            var attributes = new JArray();
            foreach(var attribute in schema.Attributes)
            {
                attributes.Add(SerializeSCIMSchemaAttribute(attribute));
            }

            jObj.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Attributes, attributes);
            return jObj;
        }

        private static JObject SerializeSCIMSchemaAttribute(SCIMSchemaAttribute scimSchemaAttribute)
        {
            var result = new JObject();
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Name, scimSchemaAttribute.Name);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Type, scimSchemaAttribute.Type.ToString().ToLowerInvariant());
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.MultiValued, scimSchemaAttribute.MultiValued);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Description, scimSchemaAttribute.Description);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Required, scimSchemaAttribute.Required);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.CaseExact, scimSchemaAttribute.CaseExact);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Mutability, scimSchemaAttribute.Mutability.ToString().ToLowerInvariant());
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Returned, scimSchemaAttribute.Returned.ToString().ToLowerInvariant());
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Uniqueness, scimSchemaAttribute.Uniqueness.ToString().ToLowerInvariant());
            if (scimSchemaAttribute.SubAttributes != null && scimSchemaAttribute.SubAttributes.Any())
            {
                var subAttributes = new JArray();
                foreach(var subAttribute in scimSchemaAttribute.SubAttributes)
                {
                    subAttributes.Add(SerializeSCIMSchemaAttribute(subAttribute));
                }

                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.SubAttributes, subAttributes);
            }

            return result;
        }
    }
}
