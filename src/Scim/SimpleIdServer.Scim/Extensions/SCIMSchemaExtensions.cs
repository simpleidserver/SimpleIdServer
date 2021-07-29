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
                { SCIMConstants.StandardSCIMRepresentationAttributes.Description,  schema.Description}
            };

            var attributes = new JArray();
            foreach(var attribute in schema.HierarchicalAttributes)
            {
                attributes.Add(SerializeSCIMSchemaAttribute(attribute));
            }

            jObj.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Attributes, attributes);
            return jObj;
        }

        private static JObject SerializeSCIMSchemaAttribute(TreeNode<SCIMSchemaAttribute> scimSchemaAttribute)
        {
            var result = new JObject();
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Name, scimSchemaAttribute.Leaf.Name);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Type, scimSchemaAttribute.Leaf.Type.ToString().ToLowerInvariant());
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.MultiValued, scimSchemaAttribute.Leaf.MultiValued);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Description, scimSchemaAttribute.Leaf.Description);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Required, scimSchemaAttribute.Leaf.Required);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.CaseExact, scimSchemaAttribute.Leaf.CaseExact);
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Mutability, scimSchemaAttribute.Leaf.Mutability.ToString().ToLowerInvariant());
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Returned, scimSchemaAttribute.Leaf.Returned.ToString().ToLowerInvariant());
            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Uniqueness, scimSchemaAttribute.Leaf.Uniqueness.ToString().ToLowerInvariant());
            if (scimSchemaAttribute.Children != null && scimSchemaAttribute.Children.Any())
            {
                var subAttributes = new JArray();
                foreach(var subAttribute in scimSchemaAttribute.Children)
                {
                    subAttributes.Add(SerializeSCIMSchemaAttribute(subAttribute));
                }

                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.SubAttributes, subAttributes);
            }

            if (scimSchemaAttribute.Leaf.CanonicalValues != null && scimSchemaAttribute.Leaf.CanonicalValues.Any())
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.CanonicalValues, new JArray(scimSchemaAttribute.Leaf.CanonicalValues));
            }

            return result;
        }
    }
}
