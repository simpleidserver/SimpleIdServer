using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkUrlAction : IWorkflowLinkAction
{
    private readonly IFormBuilderJsService _formBuidlerJsService;
    private readonly INavigationHistoryService _navigationHistoryService;

    public WorkflowLinkUrlAction(IFormBuilderJsService formBuilderJsService, INavigationHistoryService navigationHistoryService)
    {
        _formBuidlerJsService = formBuilderJsService;
        _navigationHistoryService = navigationHistoryService;
    }

    public string Type => ActionType;

    public string DisplayName => "URL";

    public static string ActionType => "URL";

    public List<string> ExcludedStepNames => new List<string>();

    public bool CanBeAppliedMultipleTimes => false;

    public async Task Execute(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        if(string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkUrlParameter>(activeLink.ActionParameter);
        await _navigationHistoryService.SaveExecutedLink(context, linkExecution.LinkId);
        await _formBuidlerJsService.Navigate(parameter.Url);
    }

    public (JsonObject json, string url)? GetRequest(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        if (string.IsNullOrWhiteSpace(activeLink.ActionParameter)) return null;
        var parameter = JsonSerializer.Deserialize<WorkflowLinkUrlParameter>(activeLink.ActionParameter);
        return (null, parameter.Url);
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink, JsonNode fakeData, WorkflowContext context)
    {
        var parameter = new WorkflowLinkUrlParameter();
        if(!string.IsNullOrWhiteSpace(workflowLink.ActionParameter))
            parameter = JsonSerializer.Deserialize<WorkflowLinkUrlParameter>(workflowLink.ActionParameter);
        builder.OpenComponent<EditWorkflowLinkComponent>(0);
        builder.AddAttribute(1, nameof(EditWorkflowLinkComponent.Parameter), parameter);
        builder.CloseComponent();
        return parameter;
    }
}
