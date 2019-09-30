using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Serialization;
using System.Net;

namespace SimpleIdServer.Scim.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult BuildError(this Controller controller, HttpStatusCode code, string scimType, string detail)
        {
            var serializer = new SCIMSerializer();
            var result = new JObject
            {
                { "status", ((int)code).ToString() },
                { "response",  serializer.Serialize(new SCIMErrorRepresentation(((int)code).ToString(), scimType, detail)) }
            };
            return new ContentResult
            {
                StatusCode = (int)code,
                Content = result.ToString(),
                ContentType = "application/json"
            };
        }
    }
}