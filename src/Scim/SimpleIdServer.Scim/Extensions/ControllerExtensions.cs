using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Serialization;
using System.Net;

namespace SimpleIdServer.Scim.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult BuildError(this Controller controller, HttpStatusCode code, string detail, string scimType = null)
        {
            var serializer = new SCIMSerializer();
            var result = new JObject
            {
                { "status", ((int)code).ToString() },
                { "response",  serializer.Serialize(new SCIMErrorRepresentation(((int)code).ToString(), detail, scimType)) }
            };
            return new ContentResult
            {
                StatusCode = (int)code,
                Content = result.ToString(),
                ContentType = "application/json"
            };
        }

        public static JObject SerializeQuery(this Controller controller)
        {
            var query = controller.Request.Query;
            var result = new JObject();
            foreach(var record in query)
            {
                result.Add(record.Key, JToken.FromObject(record));
            }

            return result;
        }
    }
}