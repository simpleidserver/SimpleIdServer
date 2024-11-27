using FormBuilder.Models.Transformer;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Transformers;

public abstract class GenericTransformer<T> : ITransformer where T : ITransformerParameters
{
    public abstract string Type { get; }

    public object Transform(string value, ITransformerParameters transformerParameters)
        => InternalTransform(value, (T)transformerParameters);

    public void BuildComponent(ITransformerParameters parameters, RenderTreeBuilder builder)
        => InternalBuild((T)parameters, builder);

    public abstract ITransformerParameters CreateEmptyInstance();

    internal abstract object InternalTransform(string value, T parameters);

    internal abstract void InternalBuild(T parameters, RenderTreeBuilder builder);
}
