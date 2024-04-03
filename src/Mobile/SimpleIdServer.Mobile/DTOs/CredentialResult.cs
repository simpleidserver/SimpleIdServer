using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class CredentialResult
    {
        [JsonPropertyName("c_nonce")]
        public string CNonce { get; set; }
        [JsonPropertyName("credential")]
        public JsonNode Credential { get; set; }
    }
}
