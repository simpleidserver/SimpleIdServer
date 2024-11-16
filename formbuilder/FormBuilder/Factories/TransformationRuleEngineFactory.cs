using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Factories;

public interface ITransformationRuleEngineFactory
{
    List<JsonNode> Transform<T>(JsonObject input, T parameter) where T : ITransformationRule;
    ITransformationRuleEngine Build(string type);
}

public class TransformationRuleEngineFactory : ITransformationRuleEngineFactory
{
    private readonly IEnumerable<ITransformationRuleEngine> _engines;

    public TransformationRuleEngineFactory(IEnumerable<ITransformationRuleEngine> engines)
    {
        _engines = engines;
    }

    public List<JsonNode> Transform<T>(JsonObject input, T parameter) where T : ITransformationRule
    {
        var engine = Build(parameter.Type);
        return engine.Transform(input, parameter);
    }

    public ITransformationRuleEngine Build(string type)
        => _engines.SingleOrDefault(e => e.Type == type);
}
