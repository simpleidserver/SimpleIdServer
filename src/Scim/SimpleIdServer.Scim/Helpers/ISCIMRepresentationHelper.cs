using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Helpers
{
    public interface ISCIMRepresentationHelper
    {
        SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, IEnumerable<SCIMSchema> schemas);
    }
}