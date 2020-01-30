// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMSchemaAttributeExtensions
    {
        public static SCIMSchemaAttribute GetAttribute(this ICollection<SCIMSchemaAttribute> attributes, List<string> lst)
        {
            if (!lst.Any())
            {
                return null;
            }

            var attr = attributes.FirstOrDefault(a => a.Name == lst.First());
            if (attr == null)
            {
                return null;
            }

            lst = lst.Skip(1).ToList();
            if (!lst.Any())
            {
                return attr;
            }

            return attr.SubAttributes.GetAttribute(lst);
        }
    }
}
