namespace FormBuilder.Components.FormElements.ListData;

public class ListDataDefinition : IFormElementDefinition
{
    public Type UiElt => typeof(ListDataElt);
    public Type RecordType => typeof(ListDataRecord);
    public string Type => TYPE;
    public static string TYPE = "ListData";
    public ElementDefinitionCategories Category => ElementDefinitionCategories.DATA;
}
