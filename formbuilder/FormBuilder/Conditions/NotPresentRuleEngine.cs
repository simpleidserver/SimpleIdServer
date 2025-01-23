using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public class NotPresentRuleEngine : GenericConditionRule<NotPresentParameter>
{
    public override string Type => NotPresentParameter.TYPE;

    protected override bool EvaluateInternal(JsonObject input, NotPresentParameter parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines)
    {
        var path = JsonPath.Parse(parameter.Source);
        var pathResult = path.Evaluate(input);
        return !pathResult.Matches.Select(m => m.Value).Where(m => !string.IsNullOrWhiteSpace(m?.ToString())).Any();
    }
}
