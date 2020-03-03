// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMSchemaExtensionModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// URI of an extended schema.
        /// </summary>
        public string Schema { get; set; }
        /// <summary>
        /// Specifies whether or not the schema extension is required for the resource type.
        /// </summary>
        public bool Required { get; set; }
    }
}
