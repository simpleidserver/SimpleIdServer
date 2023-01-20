using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.BCCallback
{
    public class BCCallbackParameter
    {
        [BindProperty(Name = BCAuthenticationResponseParameters.AuthReqId)]
        [JsonPropertyName(BCAuthenticationResponseParameters.AuthReqId)]
        public string AuthReqId { get; set; }
        [BindProperty(Name = "action")]
        [JsonPropertyName("action")]
        public BCCallbackActions Action { get; set; }
    }

    public enum BCCallbackActions
    {
        CONFIRM = 0,
        REJECT = 1
    }
}
