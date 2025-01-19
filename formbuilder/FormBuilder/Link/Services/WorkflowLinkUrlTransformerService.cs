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
    private readonly ITransformerFactory _transformerFactory;

    public WorkflowLinkUrlTransformerService(IUrlEvaluatorFactory urlEvaluatorFactory, ITransformerFactory transformerFactory)
    {
        _urlEvaluatorFactory = urlEvaluatorFactory;
        _transformerFactory = transformerFactory;
    }

    public string BuildUrl(WorkflowLinkUrlTransformationParameter parameter, JsonObject json)
    {
        if (parameter == null || json == null) return null;
        var url = parameter.Url;
        url = ParseQueryParameter(url, parameter, json);
        if(parameter.Transformers != null)
            foreach(var transformer in parameter.Transformers)
                url = _transformerFactory.Transform(url, transformer, json).ToString();

        return url;
    }
    private string ParseQueryParameter(string url, WorkflowLinkUrlTransformationParameter parameter, JsonObject json)
    {
        if (string.IsNullOrWhiteSpace(parameter.JsonSource) || string.IsNullOrWhiteSpace(parameter.QueryParameterName)) return url;
        var path = JsonPath.Parse(parameter.JsonSource);
        var pathResult = path.Evaluate(json);
        var nodes = pathResult.Matches.Select(m => m.Value);
        if (nodes.Count() != 1) return url;
        var value = nodes.Single()?.ToString();
        return _urlEvaluatorFactory.Evaluate(new DirectTargetUrl
        {
            Parameters = new Dictionary<string, string>
            {
                { parameter.QueryParameterName, value }
            },
            Url = url
        });
    }
}
