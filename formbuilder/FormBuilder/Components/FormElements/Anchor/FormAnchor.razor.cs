using FormBuilder.Components.Drag;
using FormBuilder.Factories;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public FormAnchorRecord Value { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
    [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Inject] private IWorkflowLinkActionFactory WorkflowLinkActionFactory { get; set; }

    async Task Navigate()
    {
        var link = WorkflowExecutionContext.GetLink(Value);
        if (link == null) return;
        var act = WorkflowLinkActionFactory.Build(link.ActionType);
        await act.Execute(link, WorkflowExecutionContext);
    }
}