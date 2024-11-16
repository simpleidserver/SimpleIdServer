using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Factories;
using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface IRuleEngine
{

}

public class RuleEngine : IRuleEngine
{
    private readonly ITransformerFactory _transformerFactory;
    private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;

    public RuleEngine(ITransformerFactory transformerFactory, ITransformationRuleEngineFactory transformationRuleEngineFactory)
    {
       _transformerFactory = transformerFactory;
       _transformationRuleEngineFactory = transformationRuleEngineFactory;
    }

    public void Apply(FormRecord formRecord, JsonObject input)
    {
        foreach(var elt in formRecord.Elements)
        {
            Apply(elt, input);
        }
    }

    public void Apply(dynamic elt, JsonObject input)
    {
        Apply(elt, input);
    }

    public void Apply(BaseFormLayoutRecord record, JsonObject input)
    {

    }

    public void Apply(FormStackLayoutRecord record, JsonObject input)
    {
        if (record.Elements == null) return;
        foreach (var elt in record.Elements)
            Apply(record, input);
    }

    public void Apply(BaseFormFieldRecord record, JsonObject input)
    {
        if (record.Transformation == null) return;
        var transformationResult = _transformationRuleEngineFactory.Transform(input, record.Transformation);
        // TODO : Apply the result !!! :)
    }

    public void Apply(BaseFormDataRecord record, JsonObject input)
    {
        // TODO
    }
}
