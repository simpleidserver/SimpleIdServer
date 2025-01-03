using FormBuilder.Components.Drag;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Services;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Back;

public partial class BackButton : IGenericFormElement<BackButtonRecord>
{
    [Parameter] public BackButtonRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
    [Inject] private INavigationHistoryService navigationHistoryService { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    private async Task HandleBack()
    {
        await navigationHistoryService.Back(Context);
    }
}
