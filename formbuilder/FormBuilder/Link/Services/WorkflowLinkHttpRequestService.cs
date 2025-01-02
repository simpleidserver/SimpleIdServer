using FormBuilder.Factories;
using FormBuilder.Rules;
using FormBuilder.UIs;
using System.Text.Json.Nodes;

namespace FormBuilder.Link.Services;

public interface IWorkflowLinkHttpRequestService
{
    (JsonObject json, string url) BuildUrl(WorkflowLinkHttpRequestParameter parameter, JsonObject json, AntiforgeryTokenRecord antiforgeryToken, string stepName, string workflowId, string currentLink);
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

    public (JsonObject json, string url) BuildUrl(WorkflowLinkHttpRequestParameter parameter, JsonObject json, AntiforgeryTokenRecord antiforgeryToken, string stepName, string workflowId, string currentLink)
    {
        var result = JsonObject.Parse(json.ToString()).AsObject();
        if (parameter.IsCustomParametersEnabled)
            result = _mappingRuleService.Extract(json, parameter.Rules);

        if (parameter.IsAntiforgeryEnabled && antiforgeryToken != null)
            result.Add(antiforgeryToken.FormField, antiforgeryToken.FormValue);

        var target = parameter.Target;
        if (parameter.TargetTransformer != null)
            target = _transformerFactory.Transform(parameter.Target, parameter.TargetTransformer, json)?.ToString();

        result.Add(nameof(StepViewModel.StepName), stepName);
        result.Add(nameof(StepViewModel.WorkflowId), workflowId);
        result.Add(nameof(StepViewModel.CurrentLink), currentLink);
        return (result, target);
    }
}
