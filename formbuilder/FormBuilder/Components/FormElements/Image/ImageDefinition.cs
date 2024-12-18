
namespace FormBuilder.Components.FormElements.Image;

public class ImageDefinition : IFormElementDefinition
{
    public static string TYPE => "image";

    public string Type => TYPE;

    public string Icon => "image";

    public Type UiElt => typeof(ImageComponent);

    public Type RecordType => typeof(ImageRecord);

    public ElementDefinitionCategories Category => ElementDefinitionCategories.ELEMENT;
}
