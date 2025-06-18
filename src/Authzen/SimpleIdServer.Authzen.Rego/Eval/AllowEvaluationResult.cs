using System.Text.Json.Serialization;

namespace SimpleIdServer.Authzen.Rego.Eval;

public class AllowEvaluationResult
{
    [JsonPropertyName("result")]
    public bool Result
    {
        get; set;
    }
}
