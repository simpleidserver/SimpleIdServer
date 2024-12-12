using FormBuilder.Models.Transformer;
using FormBuilder.Models.Url;
using FormBuilder.Transformers.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Transformers;

public class DirectTargetUrlTransformerParameters : ITransformerParameters
{
    public string Url { get; set; }
    public string QueryParameterName { get; set; }
    public string Type => TYPE;
    public const string TYPE = "DirectUrlTransformer";
}

public class DirectTargetUrlTransformer : GenericTransformer<DirectTargetUrlTransformerParameters>
{
    public override string Type => DirectTargetUrlTransformerParameters.TYPE;

    public override ITransformerParameters CreateEmptyInstance()
    {
        return new DirectTargetUrlTransformerParameters();
    }

    internal override object InternalTransform(string value, DirectTargetUrlTransformerParameters parameters)
    {
        return new DirectTargetUrl
        {
            Url = parameters.Url,
            Parameters = new Dictionary<string, string>
            {
                { parameters.QueryParameterName, value }
            }
        };
    }

    internal override void InternalBuild(DirectTargetUrlTransformerParameters parameters, RenderTreeBuilder builder)
    {
        builder.OpenComponent<DirectTargetUrlTransformerComponent>(0);
        builder.AddAttribute(1, nameof(DirectTargetUrlTransformerComponent.Record), parameters);
        builder.CloseComponent();
    }
}
