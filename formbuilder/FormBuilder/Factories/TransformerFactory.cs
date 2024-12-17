using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using System.Text.Json.Nodes;

namespace FormBuilder.Factories;

public interface ITransformerFactory
{
    ITransformer Build(string type);
    object Transform<T>(string value, T parameters, JsonNode data) where T : ITransformerParameters;
    List<string> GetAll();
    ITransformerParameters CreateEmptyInstance(string type);
}

public class TransformerFactory : ITransformerFactory
{
    private readonly IEnumerable<ITransformer> _transformers;

    public TransformerFactory(IEnumerable<ITransformer> transformers)
    {
        _transformers = transformers;
    }

    public ITransformer Build(string type) => _transformers.SingleOrDefault(t => t.Type == type);

    public List<string> GetAll()
        => _transformers.Select(t => t.Type).ToList();

    public object Transform<T>(string value, T parameters, JsonNode data) where T : ITransformerParameters
    {
        var transformer = Build(parameters.Type);
        return transformer.Transform(value, parameters, data);
    }

    public ITransformerParameters CreateEmptyInstance(string type)
    {
        var transformer = Build(type);
        return transformer.CreateEmptyInstance();
    }
}
