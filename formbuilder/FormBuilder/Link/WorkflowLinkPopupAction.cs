using FormBuilder.Components;
using FormBuilder.Models;
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

    public static string ActionType => "Popup";

    public async Task Execute(WorkflowLink activeLink, WorkflowExecutionContext context)
    {
        context.NextStep(activeLink);
        await _dialogService.OpenAsync<WorkflowLinkPopupActionComponent>(string.Empty, new Dictionary<string, object>
        {
            { nameof(WorkflowLinkPopupActionComponent.Context), context }
        });
    }
}