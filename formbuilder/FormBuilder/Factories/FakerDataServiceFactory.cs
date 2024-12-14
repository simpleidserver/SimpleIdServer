namespace FormBuilder.Factories;

public interface IFakerDataServiceFactory
{
    IFakerDataService Build(string formRecordName);
}

public class FakerDataServiceFactory : IFakerDataServiceFactory
{
    private readonly IEnumerable<IFakerDataService> _fakerDataServices;

    public FakerDataServiceFactory(IEnumerable<IFakerDataService> fakerDataServices)
    {
        _fakerDataServices = fakerDataServices;
    }

    public IFakerDataService Build(string formRecordName)
        => _fakerDataServices.Single(f => f.FormRecordName == formRecordName);
}
