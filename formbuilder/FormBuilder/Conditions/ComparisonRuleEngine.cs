using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public class ComparisonRuleEngine : GenericConditionRule<ComparisonParameter>
{
    public override string Type => ComparisonParameter.TYPE;

    protected override bool EvaluateInternal(JsonObject input, ComparisonParameter parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines)
    {
        var path = JsonPath.Parse(parameter.Source);
        var pathResult = path.Evaluate(input);
        var result = pathResult.Matches.Select(m => m.Value);
        if (result.Count() != 1) return false;
        var value = result.Single().ToString();
        if (parameter.Operator == ComparisonOperators.EQ) return value.Equals(parameter.Value, StringComparison.InvariantCultureIgnoreCase);
        return !value.Equals(parameter.Value, StringComparison.InvariantCultureIgnoreCase);
    }
}
