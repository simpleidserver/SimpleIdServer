using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Link.Services;
using FormBuilder.Models;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkUrlTransformerAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuilderJsService;
    private readonly IWorkflowLinkUrlTransformerService _workflowLinkUrlTransformerService;
    private readonly INavigationHistoryService _navigationHistoryService;

    public WorkflowLinkUrlTransformerAction(IFormBuilderJsService formBuilderJsService, IWorkflowLinkUrlTransformerService workflowLinkUrlTransformerService, INavigationHistoryService navigationHistoryService)
    {
        _formBuilderJsService = formBuilderJsService;
        _workflowLinkUrlTransformerService = workflowLinkUrlTransformerService;
        _navigationHistoryService = navigationHistoryService;
    }

    public static string ActionType => "UrlTransformation";

    public string Type => ActionType;

    public string DisplayName => "Url transformation";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => true;

    public async Task Execute(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        if (activeLink == null || activeLink.ActionParameter == null) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkUrlTransformationParameter>(activeLink.ActionParameter);
        var json = JsonObject.Parse(linkExecution.OutputData.ToString()).AsObject();
        var url = _workflowLinkUrlTransformerService.BuildUrl(parameter, json);
        if (string.IsNullOrWhiteSpace(url)) return;
        await _navigationHistoryService.SaveExecutedLink(context, linkExecution.LinkId);
        await _formBuilderJsService.NavigateForce(url);
    }

    public (JsonObject json, string url)? GetRequest(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return null;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkUrlTransformationParameter>(activeLink.ActionParameter);
        var json = JsonObject.Parse(linkExecution.OutputData.ToString()).AsObject();
        var url = _workflowLinkUrlTransformerService.BuildUrl(parameter, json);
        return (null, parameter.Url);
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink, JsonNode fakeData, WorkflowContext context)
    {
        var parameter = new WorkflowLinkUrlTransformationParameter();
        if(!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkUrlTransformationParameter>(workflowLink.ActionParameter);

        builder.OpenComponent<WorkflowLinkUrlTransformerComponent>(0);
        builder.AddAttribute(1, nameof(WorkflowLinkUrlTransformerComponent.Parameter), parameter);
        builder.AddAttribute(2, nameof(WorkflowLinkUrlTransformerComponent.FakeData), fakeData);
        builder.AddAttribute(3, nameof(WorkflowLinkUrlTransformerComponent.Context), context);
        builder.CloseComponent();
        return parameter;
    }
}
