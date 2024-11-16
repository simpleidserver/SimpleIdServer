using FormBuilder.Models.Transformer;

namespace FormBuilder.Transformers;

public interface ITransformer
{
    string Type { get; }
    object Transform(string value, ITransformerParameters transformerParameters);
}
