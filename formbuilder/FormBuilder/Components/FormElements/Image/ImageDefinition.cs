namespace FormBuilder.Components.FormElements.Image;

public class ImageDefinition : GenericFormElementDefinition<ImageRecord>
{
    public static string TYPE => "image";

    public override string Type => TYPE;
    public override string Icon => "image";
    public override Type UiElt => typeof(ImageComponent);
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

    protected override void ProtectedInit(ImageRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
    }
}
