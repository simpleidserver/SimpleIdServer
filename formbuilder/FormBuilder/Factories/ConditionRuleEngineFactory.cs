using FormBuilder.Conditions;

namespace FormBuilder.Factories;

public interface IConditionRuleEngineFactory
{
    IConditionRuleEngine Build(IConditionParameter parameter);
}

public class ConditionRuleEngineFactory : IConditionRuleEngineFactory
{
    private readonly IEnumerable<IConditionRuleEngine> _engines;

    public ConditionRuleEngineFactory(IEnumerable<IConditionRuleEngine> engines)
    {
        _engines = engines;
    }

    public IConditionRuleEngine Build(IConditionParameter parameter)
        => _engines.SingleOrDefault(e => e.Type == parameter.Type);
}
