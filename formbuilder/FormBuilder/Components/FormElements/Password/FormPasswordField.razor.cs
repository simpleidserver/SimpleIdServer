using FormBuilder.Components.Drag;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Password;

public partial class FormPasswordField : IGenericFormElement<FormPasswordFieldRecord>
{
    [Parameter] public FormPasswordFieldRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
    [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }
}
