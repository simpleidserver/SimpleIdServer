using FormBuilder.Models.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public abstract class GenericTransformationRule<T> : ITransformationRuleEngine where T : ITransformationRule
{
    public abstract string Type { get; }

    public List<JsonNode> Transform(JsonObject input, ITransformationRule parameter)
        => InternalTransform(input, (T)parameter);

    protected abstract List<JsonNode> InternalTransform(JsonObject input, T parameter);
}
