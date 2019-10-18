using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Uma.DTOs;
using System.Net;

namespace SimpleIdServer.Uma.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult BuildError(this Controller controller, HttpStatusCode code, string error, string errorDescription = null, string errorUri = null)
        {
            var jObj = new JObject
            {
                { UMAErrorMessageNames.Error, error }
            };
            if (!string.IsNullOrWhiteSpace(errorDescription))
            {
                jObj.Add(UMAErrorMessageNames.ErrorDescription, errorDescription);
            }

            if (!string.IsNullOrWhiteSpace(errorUri))
            {
                jObj.Add(UMAErrorMessageNames.ErrorUri, errorUri);
            }

            return new ContentResult
            {
                StatusCode = (int)code,
                ContentType = "application/json",
                Content = jObj.ToString()
            };
        }
    }
}
