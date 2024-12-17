using FormBuilder.Components;
using FormBuilder.Factories;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using FormBuilder.Rules;
using FormBuilder.Services;
using FormBuilder.UIs;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkHttpRequestAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuilderJsService;
    private readonly IMappingRuleService _mappingRuleService;
    private readonly ITransformerFactory _transformerFactory;
    private readonly FormBuilderOptions _options;

    public WorkflowLinkHttpRequestAction(IFormBuilderJsService formBuilderJsService, IMappingRuleService mappingRuleService, ITransformerFactory transformerFactory, IOptions<FormBuilderOptions> options)
    {
        _formBuilderJsService = formBuilderJsService;
        _mappingRuleService = mappingRuleService;
        _transformerFactory = transformerFactory;
        _options = options.Value;
    }

    public string Type => ActionType;

    public static string ActionType => "HttpRequest";

    public string DisplayName => "Http request";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => false;

    public async Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(activeLink.ActionParameter);
        JsonObject jObj = context.StepOutput ?? context.InputData;
        var json = context.StepOutput;
        if(parameter.IsCustomParametersEnabled)
        {
            json = _mappingRuleService.Extract(context.InputData, parameter.Rules);
        }

        if (parameter.IsAntiforgeryEnabled && context.AntiforgeryToken != null)
            json.Add(context.AntiforgeryToken.FormField, context.AntiforgeryToken.FormValue);

        var target = parameter.Target;
        if(parameter.TargetTransformer != null)
            target = _transformerFactory.Transform(parameter.Target, parameter.TargetTransformer, jObj)?.ToString();

        var currentRecord = context.GetCurrentRecord();
        json.Add(nameof(StepViewModel.StepName), currentRecord.Name);
        json.Add(nameof(StepViewModel.WorkflowId), context.Workflow.Id);
        json.Add(nameof(StepViewModel.CurrentLink), activeLink.Id);
        await _formBuilderJsService.SubmitForm(target, json, parameter.Method);
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink)
    {
        var parameter = new WorkflowLinkHttpRequestParameter();
        if (!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(workflowLink.ActionParameter);
        builder.OpenComponent<WorkflowLinkHttpRequestComponent>(0);
        builder.AddAttribute(1, nameof(WorkflowLinkHttpRequestComponent.Parameter), parameter);
        builder.CloseComponent();
        return parameter;
    }

    private Dictionary<string, string> ConvertToDic(JsonObject json)
    {
        var result = new Dictionary<string, string>();
        foreach (var kvp in json)
        {
            result.Add(kvp.Key, json[kvp.Key].ToString());
        }

        return result;
    }
}
