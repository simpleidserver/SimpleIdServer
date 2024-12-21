
namespace FormBuilder.Components.FormElements.Paragraph;

public class ParagraphDefinition : GenericFormElementDefinition<ParagraphRecord>
{

    public static string TYPE = "paragraph";
    public override string Type => TYPE;
    public override string Icon => "description";
    public override Type UiElt => typeof(ParagraphComponent);
    public override ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;

    protected override void ProtectedInit(ParagraphRecord record, WorkflowContext context, List<IFormElementDefinition> definitions)
    {
    }
}
