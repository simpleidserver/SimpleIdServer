
namespace FormBuilder.Components.FormElements.Title;

public class TitleDefinition : GenericFormElementDefinition<TitleRecord>
{

    public static string TYPE = "title";
    public override string Type => TYPE;
    public override string Icon => "title";
    public override Type UiElt => typeof(TitleComponent);
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

    protected override void ProtectedInit(TitleRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
    }
}
