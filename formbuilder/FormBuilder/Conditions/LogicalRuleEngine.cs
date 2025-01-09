using FormBuilder.Factories;
using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public class LogicalRuleEngine : GenericConditionRule<LogicalParameter>
{
    public override string Type => LogicalParameter.TYPE;

    protected override bool EvaluateInternal(JsonObject input, LogicalParameter parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines)
    {
        var leftEngine = conditionRuleEngines.Single(e => e.Type == parameter.LeftExpression.Type);
        var rightEngine = conditionRuleEngines.Single(e => e.Type == parameter.RightExpression.Type);
        if (parameter.Operator == LogicalOperators.AND) return leftEngine.Evaluate(input, parameter.LeftExpression, conditionRuleEngines) && rightEngine.Evaluate(input, parameter.RightExpression, conditionRuleEngines);
        return leftEngine.Evaluate(input, parameter.LeftExpression, conditionRuleEngines) || rightEngine.Evaluate(input, parameter.RightExpression, conditionRuleEngines);
    }
}
