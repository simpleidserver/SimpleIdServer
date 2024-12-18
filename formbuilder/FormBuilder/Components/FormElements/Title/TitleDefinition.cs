
namespace FormBuilder.Components.FormElements.Title;

public class TitleDefinition : IFormElementDefinition
{
    public string Type => TYPE;

    public static string TYPE = "title";

    public string Icon => "title";

    public Type UiElt => typeof(TitleComponent);

    public Type RecordType => typeof(TitleRecord);

    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
