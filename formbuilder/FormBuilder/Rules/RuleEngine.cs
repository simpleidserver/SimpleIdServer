using FormBuilder.Components;
using FormBuilder.Components.FormElements.Image;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Paragraph;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Components.FormElements.Title;
using FormBuilder.Factories;
using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder.Rules;

public interface IRuleEngine
{
    void Apply(WorkflowContext context);
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

    public void Apply(WorkflowContext context)
    {
        var formRecord = context.GetCurrentFormRecord();
        foreach(var elt in formRecord.Elements)
            Apply(elt, context);
    }

    public void Apply(dynamic elt, WorkflowContext context) => Apply(elt, context);

    public void Apply(BaseFormLayoutRecord record, WorkflowContext context) { }

    public void Apply(FormStackLayoutRecord record, WorkflowContext context)
    {
        if (record.Elements == null) return;
        foreach (var elt in record.Elements)
            Apply(elt, context);
    }

    public void Apply(BaseFormFieldRecord record, WorkflowContext context)
    {
        if (record.Transformation == null) return;
        var inputData = context.GetCurrentStepInputData();
        var transformationResult = _transformationRuleEngineFactory.Transform(inputData, record.Transformation);
        if (!transformationResult.Any()) return;
        record.Apply(transformationResult.First());
    }

    public void Apply(ListDataRecord record, WorkflowContext context)
    {
        if (record.RepetitionRule == null) return;
        var inputData = context.GetCurrentStepInputData();
        var result = _repetitionRuleEngineFactory.Transform(record.FieldType, inputData, record.RepetitionRule, record.Parameters);
        var tmp = new ObservableCollection<IFormElementRecord>();
        var link = context.Definition.Workflow.Links.SingleOrDefault(l => l.Source.EltId == record.Id);
        result.ForEach(r =>
        {
            var executionContext = context.GetCurrentStepExecution();
            if(link != null)
            {
                executionContext.Links.Add(new WorkflowStepLinkExecution
                {
                    Id = Guid.NewGuid().ToString(),
                    LinkId = link.Id,
                    EltId = r.Item1.Id,
                    InputData = r.Item2
                });
            }

            tmp.Add(r.Item1);
        });

        record.Elements = tmp;
    }

    public void Apply(BaseFormDataRecord record, WorkflowContext context) { }

    public void Apply(ParagraphRecord record, WorkflowContext context) { }

    public void Apply(TitleRecord record, WorkflowContext context) { }

    public void Apply(ImageRecord record, WorkflowContext context) { }
}
