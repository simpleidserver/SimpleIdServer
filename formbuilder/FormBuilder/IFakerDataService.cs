namespace FormBuilder;

public interface IFakerDataService
{
    string CorrelationId { get; }
    object Generate();
}
