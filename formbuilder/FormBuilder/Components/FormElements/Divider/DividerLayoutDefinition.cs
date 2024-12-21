namespace FormBuilder.Components.FormElements.Divider;

public class DividerLayoutDefinition : GenericFormElementDefinition<DividerLayoutRecord>
{
    public static string TYPE = "Divider";
    public override Type UiElt => typeof(DividerLayout);
    public override string Icon => "horizontal_rule";
    public override string Type => TYPE;
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.LAYOUT;

    protected override void ProtectedInit(DividerLayoutRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
    }
}
