using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface ITransformationRuleEngine
{
    string Type { get; }
    List<JsonNode> Transform(JsonObject input, ITransformationRule parameter);
}