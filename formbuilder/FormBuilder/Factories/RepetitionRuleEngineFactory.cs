using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Factories;

public interface IRepetitionRuleEngineFactory
{
    List<string> GetAll();
    IRepetitionRuleEngine Build(string type);
    List<(IFormElementRecord, JsonNode)> Transform<T>(List<string> supportedLanguageCodes, List<IFormElementDefinition> definitions, string fieldType, JsonObject jsonObj, T parameter, Dictionary<string, object> parameters) where T : IRepetitionRule;
}

public class RepetitionRuleEngineFactory : IRepetitionRuleEngineFactory
{
    private readonly IEnumerable<IRepetitionRuleEngine> _ruleEngines;

    public RepetitionRuleEngineFactory(IEnumerable<IRepetitionRuleEngine> ruleEngines)
    {
        _ruleEngines = ruleEngines;
    }

    public List<string> GetAll()
        => _ruleEngines.Select(r => r.Type).ToList();

    public IRepetitionRuleEngine Build(string type)
        => _ruleEngines.Single(r => r.Type == type);

    public List<(IFormElementRecord, JsonNode)> Transform<T>(List<string> supportedLanguageCodes, List<IFormElementDefinition> definitions, string fieldType, JsonObject jsonObj, T parameter, Dictionary<string, object> parameters) where T : IRepetitionRule
    {
        var ruleEngine = _ruleEngines.Single(r => r.Type == parameter.Type);
        return ruleEngine.Transform(supportedLanguageCodes, definitions, fieldType, jsonObj, parameter, parameters);
    }
}
