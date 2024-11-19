namespace FormBuilder;

public interface IFormElementDefinition
{
    string Type { get; }
    Type UiElt { get; }
    Type RecordType {  get; }
}
