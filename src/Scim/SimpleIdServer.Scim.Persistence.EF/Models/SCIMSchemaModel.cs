// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMSchemaModel
    {
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
        public virtual ICollection<SCIMSchemaExtensionModel> SchemaExtensions { get; set; }
        /// <summary>
        /// A complex type that defines service provider attributes and their qualities.
        /// </summary>
        public virtual ICollection<SCIMSchemaAttributeModel> Attributes { get; set; }
        public virtual ICollection<SCIMRepresentationSchemaModel> Representations { get; set; }
    }
}
