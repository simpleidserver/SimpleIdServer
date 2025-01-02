namespace FormBuilder.Factories;

public interface IFormElementTransformerFactory
{
    IFormElementTransformer Build(string name);
}

public class FormElementTransformerFactory : IFormElementTransformerFactory
{
    private readonly IEnumerable<IFormElementTransformer> _transformers;

    public FormElementTransformerFactory(IEnumerable<IFormElementTransformer> transformers)
    {
        _transformers = transformers;
    }

    public IFormElementTransformer Build(string name)
        => _transformers.SingleOrDefault(t => t.Name == name);
}
