using FormBuilder.Components;
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
    void Apply(FormRecord formRecord, JsonObject input, WorkflowExecutionContext workflowExecutionContext);
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

    public void Apply(FormRecord formRecord, JsonObject input, WorkflowExecutionContext workflowExecutionContext)
    {
        foreach(var elt in formRecord.Elements)
            Apply(elt, input, workflowExecutionContext);
    }

    public void Apply(dynamic elt, JsonObject input, WorkflowExecutionContext workflowExecutionContext) => Apply(elt, input, workflowExecutionContext);

    public void Apply(BaseFormLayoutRecord record, JsonObject input, WorkflowExecutionContext workflowExecutionContext) { }

    public void Apply(FormStackLayoutRecord record, JsonObject input, WorkflowExecutionContext workflowExecutionContext)
    {
        if (record.Elements == null) return;
        foreach (var elt in record.Elements)
            Apply(elt, input, workflowExecutionContext);
    }

    public void Apply(BaseFormFieldRecord record, JsonObject input, WorkflowExecutionContext workflowExecutionContext)
    {
        if (record.Transformation == null) return;
        var transformationResult = _transformationRuleEngineFactory.Transform(input, record.Transformation);
        if (!transformationResult.Any()) return;
        record.Apply(transformationResult.First());
    }

    public void Apply(ListDataRecord record, JsonObject input, WorkflowExecutionContext workflowExecutionContext)
    {
        if (record.RepetitionRule == null) return;
        var result = _repetitionRuleEngineFactory.Transform(record.FieldType, input, record.RepetitionRule, record.Parameters);
        var tmp = new ObservableCollection<(IFormElementRecord, WorkflowExecutionContext)>();
        var link = workflowExecutionContext.Workflow.Links.SingleOrDefault(l => l.Source.EltId == record.Id);
        result.ForEach(r =>
        {
            var copyContext = workflowExecutionContext.Clone();
            if(link != null)
            {
                copyContext.Workflow.Links.Add(new WorkflowLink
                {
                    Id = Guid.NewGuid().ToString(),
                    Source = new WorkflowLinkSource
                    {
                        EltId = r.Item1.Id
                    },
                    ActionParameter = link.ActionParameter,
                    ActionType = link.ActionType,
                    TargetStepId = link.TargetStepId,
                    SourceStepId = link.SourceStepId
                });
            }

            copyContext.RepetitionRuleData = r.Item2;
            tmp.Add((r.Item1, copyContext));
        });

        record.Elements = tmp;
    }

    public void Apply(BaseFormDataRecord record, JsonObject input, WorkflowExecutionContext workflowExecutionContext) { }

    public void Apply(ParagraphRecord record, JsonObject input, WorkflowExecutionContext workflowExecutionContext) { }
}
