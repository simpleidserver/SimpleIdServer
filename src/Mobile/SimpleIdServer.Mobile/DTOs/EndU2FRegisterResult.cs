using System.Text.Json.Serialization;

namespace SimpleIdServer.Mobile.DTOs
{
    public class EndU2FRegisterResult
    {
        [JsonPropertyName("sig")]
        public uint SignCount { get; set; }
    }
}
