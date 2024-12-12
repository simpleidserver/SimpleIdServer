namespace FormBuilder;

public interface IFakerDataService
{
    string FormRecordName { get; }
    object Generate();
}
