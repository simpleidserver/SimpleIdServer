using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Factories;

public interface IRepetitionRuleEngineFactory
{
    List<IFormElementRecord> Transform<T>(string fieldType, JsonObject jsonObj, T parameter, Dictionary<string, object> parameters) where T : IRepetitionRule;
}

public class RepetitionRuleEngineFactory : IRepetitionRuleEngineFactory
{
    private readonly IEnumerable<IRepetitionRuleEngine> _ruleEngines;

    public RepetitionRuleEngineFactory(IEnumerable<IRepetitionRuleEngine> ruleEngines)
    {
        _ruleEngines = ruleEngines;
    }

    public List<IFormElementRecord> Transform<T>(string fieldType, JsonObject jsonObj, T parameter, Dictionary<string, object> parameters) where T : IRepetitionRule
    {
        var ruleEngine = _ruleEngines.Single(r => r.Type == parameter.Type);
        return ruleEngine.Transform(fieldType, jsonObj, parameter, parameters);
    }
}
