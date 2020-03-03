// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF.Models
{
    public class SCIMSchemaAttributeModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string SchemaId { get; set; }
        /// <summary>
        /// The attribute's name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  The attribute's data type.  Valid values are "string", "boolean", "decimal", "integer", "dateTime", "reference", and "complex".
        /// </summary>
        public SCIMSchemaAttributeTypes Type { get; set; }
        /// <summary>
        /// A Boolean value indicating the attribute's plurality.
        /// </summary>
        public bool MultiValued { get; set; }
        /// <summary>
        /// The attribute's human-readable description.  When applicable, service providers MUST specify the description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A Boolean value that specifies whether or not the attribute is required.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// A collection of suggested canonical values that MAY be used(e.g., "work" and "home")
        /// </summary>
        public ICollection<string> CanonicalValues { get; set; }
        /// <summary>
        /// A Boolean value that specifies whether or not a string attribute is case sensitive.
        /// </summary>
        public bool CaseExact { get; set; }
        /// <summary>
        /// A single keyword indicating the circumstances under which the value of the attribute can be(re)defined.
        /// </summary>
        public SCIMSchemaAttributeMutabilities Mutability { get; set; }
        /// <summary>
        /// A single keyword that indicates when an attribute and associated values are returned in response to a GET request or in response to a PUT, POST, or PATCH request.
        /// </summary>
        public SCIMSchemaAttributeReturned Returned { get; set; }
        /// <summary>
        /// A single keyword value that specifies how the service provider enforces uniqueness of attribute values.
        /// </summary>
        public SCIMSchemaAttributeUniqueness Uniqueness { get; set; }
        /// <summary>
        /// A multi-valued array of JSON strings that indicate the SCIM resource types that may be referenced.
        /// </summary>
        public ICollection<string> ReferenceTypes { get; set; }
        /// <summary>
        /// Default value (string)
        /// </summary>
        public ICollection<string> DefaultValueString { get; set; }
        /// <summary>
        /// Default value (int)
        /// </summary>
        public ICollection<int> DefaultValueInt { get; set; }
        public virtual ICollection<SCIMRepresentationAttributeModel> RepresentationAttributes { get; set; }
        /// <summary>
        /// When an attribute is of type "complex", "subAttributes" defines a set of sub-attributes. "subAttributes" has the same schema sub-attributes as "attributes".
        /// </summary>
        public virtual ICollection<SCIMSchemaAttributeModel> SubAttributes { get; set; }
        public virtual SCIMSchemaAttributeModel ParentAttribute { get; set; }
        public virtual SCIMSchemaModel Schema { get; set; }
    }
}
