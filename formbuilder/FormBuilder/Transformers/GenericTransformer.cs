using FormBuilder.Models.Transformer;

namespace FormBuilder.Transformers;

public abstract class GenericTransformer<T> : ITransformer where T : ITransformerParameters
{
    public abstract string Type { get; }
    public object Transform(string value, ITransformerParameters transformerParameters)
        => InternalTransform(value, (T)transformerParameters);

    public abstract object InternalTransform(string value, T parameters);
}
