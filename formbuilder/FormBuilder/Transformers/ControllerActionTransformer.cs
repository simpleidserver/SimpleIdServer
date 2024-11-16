using FormBuilder.Factories;
using FormBuilder.Models.Transformer;
using FormBuilder.Models.Url;

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

    public string Type => ControllerActionTransformerParameters.TYPE;

    public override object InternalTransform(string value, ControllerActionTransformerParameters parameters)
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
}
