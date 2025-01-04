using FormBuilder.Models.Rules;
using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public class IncomingTokensTransformationRuleEngine : GenericTransformationRule<IncomingTokensTransformationRule>
{
    public override string Type => IncomingTokensTransformationRule.TYPE;

    protected override void ProtectedApply<R>(R record, JsonObject input, IncomingTokensTransformationRule parameter)
    {
        var path = JsonPath.Parse(parameter.Source);
        var pathResult = path.Evaluate(input);
        var result = pathResult.Matches.Select(m => m.Value).ToList().Where(r => r != null);
        if (!result.Any()) return;
        record.Apply(result.First());
    }
}
