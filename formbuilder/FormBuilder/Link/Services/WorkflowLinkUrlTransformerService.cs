using FormBuilder.Factories;
using FormBuilder.Models.Url;
using Json.Path;
using System.Text.Json.Nodes;

namespace FormBuilder.Link.Services;

public interface IWorkflowLinkUrlTransformerService
{
    string BuildUrl(WorkflowLinkUrlTransformationParameter parameter, JsonObject json);
}

public class WorkflowLinkUrlTransformerService : IWorkflowLinkUrlTransformerService
{
    private readonly IUrlEvaluatorFactory _urlEvaluatorFactory;
    public WorkflowLinkUrlTransformerService(IUrlEvaluatorFactory urlEvaluatorFactory)
    {
        _urlEvaluatorFactory = urlEvaluatorFactory;
    }

    public string BuildUrl(WorkflowLinkUrlTransformationParameter parameter, JsonObject json)
    {
        if (parameter == null || json == null || parameter.QueryParameterName == null) return null;
        if (string.IsNullOrWhiteSpace(parameter.JsonSource)) return null;
        var path = JsonPath.Parse(parameter.JsonSource);
        var pathResult = path.Evaluate(json);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1) return null;
        var value = nodes.Single()?.ToString();
        var url = _urlEvaluatorFactory.Evaluate(new DirectTargetUrl
        {
            Parameters = new Dictionary<string, string>
            {
                { parameter.QueryParameterName, value }
            },
            Url = parameter.Url
        });
        return url;
    }
}
