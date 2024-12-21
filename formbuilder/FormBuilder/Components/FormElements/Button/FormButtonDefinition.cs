namespace FormBuilder.Components.FormElements.Button;

public class FormButtonDefinition : GenericFormElementDefinition<FormButtonRecord>
{
    public static string TYPE = "Button";
    public override Type UiElt => typeof(FormButton);
    public override string Type => TYPE;
    public override string Icon => "variables";
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

    protected override void ProtectedInit(FormButtonRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
    }
}
