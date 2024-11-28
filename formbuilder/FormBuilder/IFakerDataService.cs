namespace FormBuilder;

public interface IFakerDataService
{
    Type RecordType { get; }
    object Generate();
}
