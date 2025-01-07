using FormBuilder.Factories;

namespace FormBuilder.Components.FormElements.Image;

public class ImageDefinition : GenericFormElementDefinition<ImageRecord>
{
    private readonly ITransformationRuleEngineFactory _transformationRuleEngineFactory;

    public ImageDefinition(ITransformationRuleEngineFactory transformationRuleEngineFactory)
    {
        _transformationRuleEngineFactory = transformationRuleEngineFactory;
    }

    public static string TYPE => "image";
    public override string Type => TYPE;
    public override string Icon => "image";
    public override Type UiElt => typeof(ImageComponent);
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

    protected override void ProtectedInit(ImageRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
        if (record.Transformations == null) return;
        var inputData = context.GetCurrentStepInputData();
        foreach (var transformation in record.Transformations)
            _transformationRuleEngineFactory.Apply(record, inputData, transformation);
    }
}
