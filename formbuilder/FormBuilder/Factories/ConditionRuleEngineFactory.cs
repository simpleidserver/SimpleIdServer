using FormBuilder.Conditions;
using System.Text.Json.Nodes;

namespace FormBuilder.Factories;

public interface IConditionRuleEngineFactory
{
    bool Evaluate(JsonObject input, IConditionParameter parameter);
}

public class ConditionRuleEngineFactory : IConditionRuleEngineFactory
{
    private readonly IEnumerable<IConditionRuleEngine> _engines;

    public ConditionRuleEngineFactory(IEnumerable<IConditionRuleEngine> engines)
    {
        _engines = engines;
    }

    public bool Evaluate(JsonObject input, IConditionParameter parameter)
        => _engines.Single(e => e.Type == parameter.Type).Evaluate(input, parameter, _engines);
}
