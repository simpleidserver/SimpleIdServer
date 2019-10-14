using System.Collections.Generic;

namespace SimpleIdServer.Scim
{
    public class SCIMHostOptions
    {
        public SCIMHostOptions()
        {
            UserSchemasIds = new List<string> { "urn:ietf:params:scim:schemas:core:2.0:User" };
            SCIMIdClaimName = "scim_id";
            MaxOperations = 1000;
            MaxPayloadSize = 1048576;
            MaxResults = 200;
        }

        /// <summary>
        /// User schemas URLS.
        /// </summary>
        public ICollection<string> UserSchemasIds { get; set; }
        /// <summary>
        /// Group schemas URLS.
        /// </summary>
        public ICollection<string> GroupSchemaIds { get; set; }
        /// <summary>
        /// Name of the claim used to get the scim identifier.
        /// </summary>
        public string SCIMIdClaimName { get; set; }
        /// <summary>
        /// An integer value specifying the maximum number of operations.
        /// </summary>
        public int MaxOperations { get; set; }
        /// <summary>
        /// An integer value specifying the maximum payload size in bytes.
        /// </summary>
        public int MaxPayloadSize { get; set; }
        /// <summary>
        /// An integer value specifying the maximum number of resources returned in a response.
        /// </summary>
        public int MaxResults { get; set; }
    }
}
