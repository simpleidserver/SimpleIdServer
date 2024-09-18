using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Host.Acceptance.Tests;

public class JwksResult
{
    [JsonPropertyName("keys")]
    public ICollection<JsonObject> JsonWebKeys { get; set; } = new List<JsonObject>();
}
