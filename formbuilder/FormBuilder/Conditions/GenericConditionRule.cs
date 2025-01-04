using System.Text.Json.Nodes;

namespace FormBuilder.Conditions;

public abstract class GenericConditionRule<T> : IConditionRuleEngine where T : IConditionParameter
{
    public abstract string Type { get; }

    public bool Evaluate(JsonObject input, IConditionParameter parameter)
        => EvaluateInternal(input, (T)parameter);

    protected abstract bool EvaluateInternal(JsonObject input, T parameter);
}
