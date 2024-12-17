using FormBuilder.Models.Transformer;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json.Nodes;

namespace FormBuilder.Transformers;

public abstract class GenericTransformer<T> : ITransformer where T : ITransformerParameters
{
    public abstract string Type { get; }

    public object Transform(string value, ITransformerParameters transformerParameters, JsonNode data)
        => InternalTransform(value, (T)transformerParameters, data);

    public void BuildComponent(ITransformerParameters parameters, RenderTreeBuilder builder)
        => InternalBuild((T)parameters, builder);

    public abstract ITransformerParameters CreateEmptyInstance();

    internal abstract object InternalTransform(string value, T parameters, JsonNode data);

    internal abstract void InternalBuild(T parameters, RenderTreeBuilder builder);
}
