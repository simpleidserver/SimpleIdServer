using FormBuilder.Models.Rules;
using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public class IncomingTokensTransformationRuleEngine : GenericTransformationRule<IncomingTokensTransformationRule>
{
    public override string Type => IncomingTokensTransformationRule.TYPE;

    protected override List<JsonNode> InternalTransform(JsonObject input, IncomingTokensTransformationRule parameter)
    {
        var path = JsonPath.Parse(parameter.Source);
        var result = path.Evaluate(input);
        return result.Matches.Select(m => m.Value).ToList();
    }
}
