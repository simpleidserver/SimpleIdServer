// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMRepresentationAttributeExtensions
    {
        public static SCIMRepresentationAttribute GetAttribute(this ICollection<SCIMRepresentationAttribute> attributes, List<string> lst)
        {
            if (!lst.Any())
            {
                return null;
            }

            var attr = attributes.FirstOrDefault(a => a.SchemaAttribute.Name == lst.First());
            if (attr == null)
            {
                return null;
            }

            lst = lst.Skip(1).ToList();
            if (!lst.Any())
            {
                return attr;
            }

            return attr.Values.GetAttribute(lst);
        }

        public static void GetAttributesByAttrSchemaId(this ICollection<SCIMRepresentationAttribute> attributes, string attrSchemaId, ICollection<SCIMRepresentationAttribute> result)
        {
            if (!attributes.Any())
            {
                return;
            }

            var attr = attributes.FirstOrDefault(a => a.SchemaAttribute.Id == attrSchemaId);
            if (attr != null)
            {
                result.Add(attr);
            }

            var subAttributes = attributes.SelectMany(a => a.Values).ToList();
            subAttributes.GetAttributesByAttrSchemaId(attrSchemaId, result);
        }
    }
}
