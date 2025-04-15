using FormBuilder.Factories;
using FormBuilder.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.ListData;

public class ListDataDefinition : GenericFormElementDefinition<ListDataRecord>
{
    private readonly IRepetitionRuleEngineFactory _repetitionRuleEngineFactory;
    private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;

    public ListDataDefinition(IRepetitionRuleEngineFactory repetitionRuleEngineFactory, ITransformationRuleEngineFactory transformationRuleEngineFactory)
    {
        _repetitionRuleEngineFactory = repetitionRuleEngineFactory;
        _transformationRuleEngineFactory = transformationRuleEngineFactory;
    }

    public static string TYPE = "ListData";
    public override Type UiElt => typeof(ListDataElt);
    public override string Type => TYPE;
    public override string Icon => "list";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.DATA;

    protected override void ProtectedInit(ListDataRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
        var inputData = context.GetCurrentStepInputData();
        if (record.RepetitionRule == null || inputData == null) return;
        var link = context.Definition.Workflow.Links.SingleOrDefault(l => l.Source.EltId == record.Id);
        if (!inputData.ContainsKey("LayoutCurrentLink") && link != null)
        {
            inputData.Add("LayoutCurrentLink", link.Id);
        }

        var result = _repetitionRuleEngineFactory.Transform(context.Execution.SupportedLanguageCodes, definitions, record.FieldType, inputData, record.RepetitionRule, record.Parameters);
        ApplyChildren(record, result, definitions, context);
        ApplyTransformations(record, result);
        ApplyHtmlAttributes(record, result);
        var tmp = new ObservableCollection<IFormElementRecord>();
        result.ForEach(r =>
        {
            var executionContext = context.GetCurrentStepExecution();
            if (link != null)
            {
                executionContext.Links.Add(new WorkflowStepLinkExecution
                {
                    Id = Guid.NewGuid().ToString(),
                    LinkId = link.Id,
                    EltId = r.Item1.Id,
                    InputData = r.Item2,
                    OutputData = r.Item2
                });
            }

            tmp.Add(r.Item1);
        });

        record.Elements = tmp;
    }

    private void ApplyChildren(ListDataRecord listDataRecord, List<(IFormElementRecord, JsonNode)> records, List<IFormElementDefinition> definitions, WorkflowContext context)
    {
        if (listDataRecord == null) return;
        foreach (var record in records)
        {
            var layoutRecord = record.Item1 as BaseFormLayoutRecord;
            if (layoutRecord == null) continue;
            var children = new ObservableCollection<IFormElementRecord>();
            foreach (var child in listDataRecord.Children)
            {
                var newContext = context.BuildNewContext(record.Item2.AsObject());
                var definition = definitions.Single(d => d.Type == child.Type);
                definition.Init(child, newContext, definitions);
                children.Add(child);
            }

            layoutRecord.Elements = children;
        }
    }

    private void ApplyTransformations(ListDataRecord listDataRecord, List<(IFormElementRecord, JsonNode)> records)
    {
        if (listDataRecord.Transformations == null || !listDataRecord.Transformations.Any()) return;
        foreach (var record in records)
        {
            var inputData = record.Item2.AsObject();
            foreach (var transformation in listDataRecord.Transformations)
            {
                _transformationRuleEngineFactory.Apply(record.Item1, inputData, transformation);
            }
        }
    }

    private void ApplyHtmlAttributes(ListDataRecord listDataRecord, List<(IFormElementRecord, JsonNode)> records)
    {
        if (listDataRecord.HtmlAttributes == null || !listDataRecord.HtmlAttributes.Any()) return;
        foreach(var record in records)
        {
            record.Item1.HtmlAttributes = listDataRecord.HtmlAttributes;
        }
    }
}
