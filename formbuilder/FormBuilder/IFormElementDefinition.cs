namespace FormBuilder;

public interface IFormElementDefinition
{
    string Type { get; }
    string Icon { get; }
    Type UiElt { get; }
    Type RecordType {  get; }
    ElementDefinitionCategories Category { get; }
}
