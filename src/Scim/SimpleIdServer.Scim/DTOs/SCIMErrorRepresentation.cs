using SimpleIdServer.Scim.Serialization;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.DTOs
{
    [SCIMSchema("urn:ietf:params:scim:api:messages:2.0:Error")]
    public class SCIMErrorRepresentation
    {
        public SCIMErrorRepresentation(string status, string scimType, string detail)
        {
            Status = status;
            ScimType = scimType;
            Detail = detail;
        }

        [SCIMSchemaProperty("status")]
        public string Status { get; set; }
        [SCIMSchemaProperty("scimType")]
        public string ScimType { get; set; }
        [SCIMSchemaProperty("detail")]
        public string Detail { get; set; }
    }
}