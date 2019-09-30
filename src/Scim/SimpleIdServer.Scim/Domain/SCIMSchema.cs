using System;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Domain
{
    public class SCIMSchema : ICloneable
    {
        public SCIMSchema()
        {
            Attributes = new List<SCIMSchemaAttribute>();
        }

        /// <summary>
        ///  The unique URI of the schema.  When applicable, service providers MUST specify the URI, e.g., "urn:ietf:params:scim:schemas:core:2.0:User"
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The schema's human-readable name. 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///   The schema's human-readable description. 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///   A complex type that defines service provider attributes and their qualities.
        /// </summary>
        public ICollection<SCIMSchemaAttribute> Attributes { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
