using System.Collections.Generic;

namespace SimpleIdServer.Scim
{
    public class SCIMHostOptions
    {
        public SCIMHostOptions()
        {
            UserSchemasIds = new List<string> { "urn:ietf:params:scim:schemas:core:2.0:User" };
        }

        /// <summary>
        /// User schemas URLS.
        /// </summary>
        public ICollection<string> UserSchemasIds { get; set; }
    }
}
