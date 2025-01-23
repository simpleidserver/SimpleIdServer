using FormBuilder.Factories;
using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public class LogicalRuleEngine : GenericConditionRule<LogicalParameter>
{
    public override string Type => LogicalParameter.TYPE;

    protected override bool EvaluateInternal(JsonObject input, LogicalParameter parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines)
    {
        var leftCondition = conditionRuleEngines.Single(e => e.Type == parameter.LeftExpression.Type).Evaluate(input, parameter.LeftExpression, conditionRuleEngines);
        var rightCondition = conditionRuleEngines.Single(e => e.Type == parameter.RightExpression.Type).Evaluate(input, parameter.RightExpression, conditionRuleEngines);
        bool result = false;
        if (parameter.Operator == LogicalOperators.AND) result = leftCondition == true && rightCondition == true;
        else result = leftCondition || rightCondition;
        return result;
    }
}
