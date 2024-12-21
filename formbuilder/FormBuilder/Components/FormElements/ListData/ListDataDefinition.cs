using FormBuilder.Factories;
using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder.Components.FormElements.ListData;

public class ListDataDefinition : GenericFormElementDefinition<ListDataRecord>
{
    private readonly IRepetitionRuleEngineFactory _repetitionRuleEngineFactory;

    public ListDataDefinition(IRepetitionRuleEngineFactory repetitionRuleEngineFactory)
    {
        _repetitionRuleEngineFactory = repetitionRuleEngineFactory;
    }

    public static string TYPE = "ListData";
    public override Type UiElt => typeof(ListDataElt);
    public override string Type => TYPE;
    public override string Icon => "list";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.DATA;

    protected override void ProtectedInit(ListDataRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
        if (record.RepetitionRule == null) return;
        var inputData = context.GetCurrentStepInputData();
        var result = _repetitionRuleEngineFactory.Transform(definitions, record.FieldType, inputData, record.RepetitionRule, record.Parameters);
        var tmp = new ObservableCollection<IFormElementRecord>();
        var link = context.Definition.Workflow.Links.SingleOrDefault(l => l.Source.EltId == record.Id);
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
}
