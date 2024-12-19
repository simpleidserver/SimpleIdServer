using FormBuilder.Components;
using FormBuilder.Link.Components;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Radzen;

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

    public async Task Execute(WorkflowLink activeLink, WorkflowContext context)
    {
        var newContext = context.BuildContextAndMoveToStep(activeLink.TargetStepId);
        await _dialogService.OpenAsync<WorkflowLinkPopupActionComponent>(string.Empty, new Dictionary<string, object>
        {
            { nameof(WorkflowLinkPopupActionComponent.Context), newContext }
        });
    }

    public object Render(RenderTreeBuilder builder, WorkflowLink workflowLink)
    {
        return null;
    }
}