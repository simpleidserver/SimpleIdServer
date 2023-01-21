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
        public int Action { get; set; }
        [JsonIgnore]
        public BCCallbackActions ActionEnum
        {
            get
            {
                return (BCCallbackActions)Action;
            }
            set
            {
                Action = (int)value;   
            }
        }
    }

    public enum BCCallbackActions
    {
        CONFIRM = 0,
        REJECT = 1
    }
}
