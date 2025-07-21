using FormBuilder.Factories;
using FormBuilder.Rules;
using FormBuilder.UIs;
using System.Text.Json.Nodes;

namespace FormBuilder.Link.Services;

public interface IWorkflowLinkHttpRequestService
{
    (JsonObject json, string url) BuildUrl(WorkflowLinkHttpRequestParameter parameter, JsonObject json, AntiforgeryTokenRecord antiforgeryToken, string stepId, string workflowId, string currentLink);
}

public class WorkflowLinkHttpRequestService : IWorkflowLinkHttpRequestService
{
    private readonly ITransformerFactory _transformerFactory;
    private readonly IMappingRuleService _mappingRuleService;

    public WorkflowLinkHttpRequestService(ITransformerFactory transformerFactory, IMappingRuleService mappingRuleService)
    {
        _transformerFactory = transformerFactory;
        _mappingRuleService = mappingRuleService;
    }

    public (JsonObject json, string url) BuildUrl(WorkflowLinkHttpRequestParameter parameter, JsonObject json, AntiforgeryTokenRecord antiforgeryToken, string stepId, string workflowId, string currentLink)
    {
        var result = JsonObject.Parse(json.ToString()).AsObject();
        if (parameter.IsCustomParametersEnabled)
            result = _mappingRuleService.Extract(json, parameter.Rules);

        if (parameter.IsAntiforgeryEnabled && antiforgeryToken != null)
            result.Add(antiforgeryToken.FormField, antiforgeryToken.FormValue);

        var target = parameter.Target;
        if (parameter.Transformers != null)
        {
            foreach (var transformer in parameter.Transformers)
                target = _transformerFactory.Transform(target, transformer, json)?.ToString();
        }

        if (Uri.TryCreate(target, UriKind.RelativeOrAbsolute, out var uri))
        {
            var query = uri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var queryParams = System.Web.HttpUtility.ParseQueryString(query);
                foreach (string key in queryParams.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        result[key] = queryParams[key];
                    }
                }

                target = uri.IsAbsoluteUri
                    ? $"{uri.Scheme}://{uri.Authority}{uri.AbsolutePath}"
                    : uri.GetLeftPart(UriPartial.Path);
            }
        }

        result.Add(nameof(IStepViewModel.StepId), stepId);
        result.Add(nameof(IStepViewModel.WorkflowId), workflowId);
        result.Add(nameof(IStepViewModel.CurrentLink), currentLink);
        return (result, target);
    }
}
