namespace FormBuilder.Factories;

public interface IFakerDataServiceFactory
{
    IFakerDataService Build(Type type);
}

public class FakerDataServiceFactory : IFakerDataServiceFactory
{
    private readonly IEnumerable<IFakerDataService> _fakerDataServices;

    public FakerDataServiceFactory(IEnumerable<IFakerDataService> fakerDataServices)
    {
        _fakerDataServices = fakerDataServices;
    }

    public IFakerDataService Build(Type type)
        => _fakerDataServices.Single(f => f.RecordType == type);
}
