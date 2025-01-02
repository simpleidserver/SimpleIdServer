using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Link.Services;
using FormBuilder.Models;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkHttpRequestAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuilderJsService;
    private readonly IWorkflowLinkHttpRequestService _workflowLinkHttpRequestService;
    private readonly FormBuilderOptions _options;

    public WorkflowLinkHttpRequestAction(IFormBuilderJsService formBuilderJsService, IWorkflowLinkHttpRequestService workflowLinkHttpRequestService, IOptions<FormBuilderOptions> options)
    {
        _formBuilderJsService = formBuilderJsService;
        _workflowLinkHttpRequestService = workflowLinkHttpRequestService;
        _options = options.Value;
    }

    public string Type => ActionType;

    public static string ActionType => "HttpRequest";

    public string DisplayName => "Http request";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => false;

    public async Task Execute(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(activeLink.ActionParameter);
        var currentRecord = context.GetCurrentFormRecord();
        var result = _workflowLinkHttpRequestService.BuildUrl(parameter, linkExecution.OutputData.AsObject(), context.Execution.AntiforgeryToken, currentRecord.Name, context.Definition.Workflow.Id, activeLink.Id);
        await _formBuilderJsService.SubmitForm(result.url, result.json, parameter.Method);
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink, JsonNode fakeData, WorkflowContext context)
    {
        var parameter = new WorkflowLinkHttpRequestParameter();
        if (!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkHttpRequestParameter>(workflowLink.ActionParameter);
        builder.OpenComponent<WorkflowLinkHttpRequestComponent>(0);
        builder.AddAttribute(1, nameof(WorkflowLinkHttpRequestComponent.Parameter), parameter);
        builder.AddAttribute(2, nameof(WorkflowLinkHttpRequestComponent.FakeData), fakeData);
        builder.AddAttribute(3, nameof(WorkflowLinkHttpRequestComponent.Context), context);
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
