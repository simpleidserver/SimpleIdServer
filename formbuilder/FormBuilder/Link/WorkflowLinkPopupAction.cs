using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Radzen;
using System.Text.Json.Nodes;

namespace FormBuilder.Link;

public class WorkflowLinkPopupAction : IWorkflowLinkAction
{
    private readonly DialogService _dialogService;

    public WorkflowLinkPopupAction(DialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public string Type => ActionType;

    public string DisplayName => "Popup";

    public static string ActionType => "Popup";

    public List<string> ExcludedStepNames => new List<string>
    {
        Constants.EmptyStep.Name
    };

    public bool CanBeAppliedMultipleTimes => false;

    public async Task Execute(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
    {
        var newContext = context.BuildContextAndMoveToStep(activeLink.TargetStepId);
        await _dialogService.OpenAsync<WorkflowLinkPopupActionComponent>(string.Empty, new Dictionary<string, object>
        {
            { nameof(WorkflowLinkPopupActionComponent.Context), newContext }
        });
    }

    public (JsonObject json, string url)? GetRequest(WorkflowLink activeLink, WorkflowStepLinkExecution linkExecution, WorkflowContext context)
        => null;

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink, JsonNode fakeData, WorkflowContext context)
    {
        return null;
    }
}