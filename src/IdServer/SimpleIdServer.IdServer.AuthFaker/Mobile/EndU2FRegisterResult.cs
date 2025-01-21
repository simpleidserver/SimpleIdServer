using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.AuthFaker.Mobile;

internal class EndU2FRegisterResult
{
    [JsonPropertyName("sig")]
    public uint SignCount { get; set; }
}
