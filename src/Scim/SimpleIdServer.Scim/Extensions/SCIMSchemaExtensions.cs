// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMSchemaExtensions
    {
        public static JObject ToResponse(this SCIMSchema schema)
        {
            var jObj = new JObject
            {
                { StandardSCIMRepresentationAttributes.Id, schema.Id },
                { StandardSCIMRepresentationAttributes.Name,  schema.Name},
                { StandardSCIMRepresentationAttributes.Description,  schema.Description}
            };

            var attributes = new JArray();
            schema.AddCommonAttributes();
            foreach(var attribute in schema.HierarchicalAttributes)
            {
                attributes.Add(SerializeSCIMSchemaAttribute(attribute));
            }

            jObj.Add(StandardSCIMRepresentationAttributes.Attributes, attributes);
            return jObj;
        }

        private static JObject SerializeSCIMSchemaAttribute(TreeNode<SCIMSchemaAttribute> scimSchemaAttribute)
        {
            var result = new JObject();
            result.Add(StandardSCIMRepresentationAttributes.Name, scimSchemaAttribute.Leaf.Name);
            result.Add(StandardSCIMRepresentationAttributes.Type, scimSchemaAttribute.Leaf.Type.ToString().ToLowerInvariant());
            result.Add(StandardSCIMRepresentationAttributes.MultiValued, scimSchemaAttribute.Leaf.MultiValued);
            result.Add(StandardSCIMRepresentationAttributes.Description, scimSchemaAttribute.Leaf.Description);
            result.Add(StandardSCIMRepresentationAttributes.Required, scimSchemaAttribute.Leaf.Required);
            result.Add(StandardSCIMRepresentationAttributes.CaseExact, scimSchemaAttribute.Leaf.CaseExact);
            result.Add(StandardSCIMRepresentationAttributes.Mutability, scimSchemaAttribute.Leaf.Mutability.ToName());
            result.Add(StandardSCIMRepresentationAttributes.Returned, scimSchemaAttribute.Leaf.Returned.ToName());
            result.Add(StandardSCIMRepresentationAttributes.Uniqueness, scimSchemaAttribute.Leaf.Uniqueness.ToName());
            if (scimSchemaAttribute.Children != null && scimSchemaAttribute.Children.Any())
            {
                var subAttributes = new JArray();
                foreach(var subAttribute in scimSchemaAttribute.Children)
                {
                    subAttributes.Add(SerializeSCIMSchemaAttribute(subAttribute));
                }

                result.Add(StandardSCIMRepresentationAttributes.SubAttributes, subAttributes);
            }

            if (scimSchemaAttribute.Leaf.CanonicalValues != null && scimSchemaAttribute.Leaf.CanonicalValues.Any())
            {
                result.Add(StandardSCIMRepresentationAttributes.CanonicalValues, new JArray(scimSchemaAttribute.Leaf.CanonicalValues));
            }

            return result;
        }
    }
}
