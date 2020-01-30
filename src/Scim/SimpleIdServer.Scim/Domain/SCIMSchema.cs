// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMSchema : ICloneable
    {
        public SCIMSchema()
        {
            Attributes = new List<SCIMSchemaAttribute>();
            SchemaExtensions = new List<SCIMSchemaExtension>();
        }

        /// <summary>
        /// The unique URI of the schema. When applicable, service providers MUST specify the URI, e.g., "urn:ietf:params:scim:schemas:core:2.0:User".
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The schema's human-readable name. 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The schema's human-readable description. 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Is root schema.
        /// </summary>
        public bool IsRootSchema { get; set; }
        /// <summary>
        /// Resource type.
        /// </summary>
        public string ResourceType { get; set; }
        /// <summary>
        /// Gets or sets the schema extensions.
        /// </summary>
        public ICollection<SCIMSchemaExtension> SchemaExtensions { get; set; }
        /// <summary>
        /// A complex type that defines service provider attributes and their qualities.
        /// </summary>
        public virtual ICollection<SCIMSchemaAttribute> Attributes { get; set; }

        public SCIMSchemaAttribute GetAttribute(string path)
        {
            var splitted = path.Split('.');
            return Attributes.GetAttribute(splitted.ToList());
        }

        public object Clone()
        {
            return new SCIMSchema
            {
                Id = Id,
                Name = Name,
                Description = Description,
                IsRootSchema = IsRootSchema,
                Attributes = Attributes.Select(a => (SCIMSchemaAttribute)a.Clone()).ToList(),
                SchemaExtensions = SchemaExtensions.Select(a => (SCIMSchemaExtension)a.Clone()).ToList()
            };
        }
    }
}
