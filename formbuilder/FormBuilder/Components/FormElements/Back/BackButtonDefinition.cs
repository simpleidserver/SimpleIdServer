namespace FormBuilder.Components.FormElements.Back;

public class BackButtonDefinition : GenericFormElementDefinition<BackButtonRecord>
{
    public static string TYPE = "BackButton";
    public override Type UiElt => typeof(BackButton);
    public override string Type => TYPE;
    public override string Icon => "arrow_back";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

    protected override void ProtectedInit(BackButtonRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
    }
}
