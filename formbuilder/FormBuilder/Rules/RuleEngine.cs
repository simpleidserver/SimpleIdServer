using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Paragraph;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Factories;
using FormBuilder.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace FormBuilder.Rules;

public interface IRuleEngine
{
    void Apply(FormRecord formRecord, JsonObject input);
}

public class RuleEngine : IRuleEngine
{
    private readonly ITransformerFactory _transformerFactory;
    private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;
    private readonly IRepetitionRuleEngineFactory _repetitionRuleEngineFactory;

    public RuleEngine(ITransformerFactory transformerFactory, ITransformationRuleEngineFactory transformationRuleEngineFactory, IRepetitionRuleEngineFactory repetitionRuleEngineFactory)
    {
       _transformerFactory = transformerFactory;
       _transformationRuleEngineFactory = transformationRuleEngineFactory;
       _repetitionRuleEngineFactory = repetitionRuleEngineFactory;
    }

    public void Apply(FormRecord formRecord, JsonObject input)
    {
        foreach(var elt in formRecord.Elements)
            Apply(elt, input);
    }

    public void Apply(dynamic elt, JsonObject input) => Apply(elt, input);

    public void Apply(BaseFormLayoutRecord record, JsonObject input) { }

    public void Apply(FormStackLayoutRecord record, JsonObject input)
    {
        if (record.Elements == null) return;
        foreach (var elt in record.Elements)
            Apply(elt, input);
    }

    public void Apply(BaseFormFieldRecord record, JsonObject input)
    {
        if (record.Transformation == null) return;
        var transformationResult = _transformationRuleEngineFactory.Transform(input, record.Transformation);
        if (!transformationResult.Any()) return;
        record.Apply(transformationResult.First());
    }

    public void Apply(ListDataRecord record, JsonObject input)
    {
        if (record.RepetitionRule == null) return;
        var elts = _repetitionRuleEngineFactory.Transform(record.FieldType, input, record.RepetitionRule, record.Parameters);
        record.Elements = new ObservableCollection<IFormElementRecord>(elts);
    }

    public void Apply(BaseFormDataRecord record, JsonObject input)
    {
    }

    public void Apply(ParagraphRecord record, JsonObject input) { }
}
