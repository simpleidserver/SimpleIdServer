using FormBuilder.Components.Drag;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Divider;

public partial class DividerLayout : IGenericFormElement<DividerLayoutRecord>
{
    [Parameter] public DividerLayoutRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
    [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
}