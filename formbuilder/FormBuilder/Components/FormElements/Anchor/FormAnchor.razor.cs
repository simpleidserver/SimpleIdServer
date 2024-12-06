using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using FormBuilder.Link;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    ElementReference LinkElt;
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public FormAnchorRecord Value { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
    [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Inject] private ITargetUrlHelperFactory targetUrlHelperFactory { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IEnumerable<IWorkflowLinkAction> Actions { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }

    async Task Navigate()
    {
        var link = WorkflowExecutionContext.GetLink(Value);
        if (link == null) return;
        var act = Actions.Single(a => a.Type == link.ActionType);
        await act.Execute(link, WorkflowExecutionContext);
    }
}