namespace FormBuilder;

public interface IFormElementDefinition
{
    Type UiElt { get; }
    Type RecordType {  get; }
}
