using FormBuilder.Factories;
using FormBuilder.Models.Transformer;
using FormBuilder.Models.Url;
using FormBuilder.Transformers.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FormBuilder.Transformers;

public class ControllerActionTransformerParameters : ITransformerParameters
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public string QueryParameterName { get; set; }
    public string Type => TYPE;
    public const string TYPE = "ControllerActionTransformer";
}

public class ControllerActionTransformer : GenericTransformer<ControllerActionTransformerParameters>
{
    private readonly ITargetUrlHelperFactory _targetUrlHelperFactory;

    public ControllerActionTransformer(ITargetUrlHelperFactory targetUrlHelperFactory)
    {
        _targetUrlHelperFactory = targetUrlHelperFactory;
    }

    public override string Type => ControllerActionTransformerParameters.TYPE;

    public override ITransformerParameters CreateEmptyInstance()
    {
        return new ControllerActionTransformerParameters();
    }

    internal override object InternalTransform(string value, ControllerActionTransformerParameters parameters)
    {
        return new ControllerActionTargetUrl
        {
            Action = parameters.Action,
            Controller = parameters.Controller,
            Parameters = new Dictionary<string, string>
            {
                { parameters.QueryParameterName, value }
            }
        };
    }

    internal override void InternalBuild(ControllerActionTransformerParameters parameters, RenderTreeBuilder builder)
    {
        builder.OpenComponent<ControllerActionTransformerComponent>(0);
        builder.AddAttribute(1, nameof(ControllerActionTransformerComponent.Record), parameters);
        builder.CloseComponent();
    }
}
