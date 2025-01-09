using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public abstract class GenericConditionRule<T> : IConditionRuleEngine where T : IConditionParameter
{
    public abstract string Type { get; }

    public bool Evaluate(JsonObject input, IConditionParameter parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines)
        => EvaluateInternal(input, (T)parameter, conditionRuleEngines);

    protected abstract bool EvaluateInternal(JsonObject input, T parameter, IEnumerable<IConditionRuleEngine> conditionRuleEngines);
}
