using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;

namespace FormBuilder.Factories;

public interface ITransformerFactory
{
    ITransformer Build(string type);
    object Transform<T>(string value, T parameters) where T : ITransformerParameters;
}

internal class TransformerFactory : ITransformerFactory
{
    private readonly IEnumerable<ITransformer> _transformers;

    public TransformerFactory(IEnumerable<ITransformer> transformers)
    {
        _transformers = transformers;
    }

    public ITransformer Build(string type) => _transformers.SingleOrDefault(t => t.Type == type);

    public object Transform<T>(string value, T parameters) where T : ITransformerParameters
    {
        var transformer = Build(parameters.Type);
        return transformer.Transform(value, parameters);
    }
}
