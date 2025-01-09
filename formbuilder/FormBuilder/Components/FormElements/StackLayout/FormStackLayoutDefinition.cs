using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutDefinition : GenericFormElementDefinition<FormStackLayoutRecord>
{
    private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;

    public FormStackLayoutDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory)
    {
        _transformationRuleEngineFactory = transformationRuleEngineFactory;
    }

    public static string TYPE = "Stacklayout";
    public override Type UiElt => typeof(FormStackLayout);
    public override string Type => TYPE;
    public override string Icon => "view_agenda";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.LAYOUT;

    protected override void ProtectedInit(FormStackLayoutRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
        if (record.Elements != null)
        {
            foreach (var elt in record.Elements)
            {
                var definition = definitions.Single(d => d.Type == elt.Type);
                definition.Init(elt, context, definitions);
            }
        }

        if (record.Transformations == null) return;
        var inputData = context.GetCurrentStepInputData();
        foreach (var transformation in record.Transformations)
            _transformationRuleEngineFactory.Apply(record, inputData, transformation);
    }
}
