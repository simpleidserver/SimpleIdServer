using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System.Text.Json.Nodes;

namespace FormBuilder.Factories;

public interface ITransformationRuleEngineFactory
{
    void Apply<T, R>(R record, JsonObject input, T parameter) where T : ITransformationRule where R : BaseFormFieldRecord;
    ITransformationRuleEngine Build(string type);
}

public class TransformationRuleEngineFactory : ITransformationRuleEngineFactory
{
    private readonly IEnumerable<ITransformationRuleEngine> _engines;

    public TransformationRuleEngineFactory(IEnumerable<ITransformationRuleEngine> engines)
    {
        _engines = engines;
    }

    public void Apply<T, R>(R record, JsonObject input, T parameter) where T : ITransformationRule where R : BaseFormFieldRecord
    {
        var engine = Build(parameter.Type);
        engine.Apply(record, input, parameter);
    }

    public ITransformationRuleEngine Build(string type)
        => _engines.SingleOrDefault(e => e.Type == type);
}
