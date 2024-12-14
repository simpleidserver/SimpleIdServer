
namespace FormBuilder.Components.FormElements.Paragraph;

public class ParagraphDefinition : IFormElementDefinition
{
    public string Type => TYPE;

    public static string TYPE = "paragraph";

    public string Icon => "description";

    public Type UiElt => typeof(ParagraphComponent);

    public Type RecordType => typeof(ParagraphRecord);

    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
