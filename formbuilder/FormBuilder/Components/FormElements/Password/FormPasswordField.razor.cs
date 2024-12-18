using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Password;

public partial class FormPasswordField : IGenericFormElement<FormPasswordFieldRecord>
{
    [Parameter] public FormPasswordFieldRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
}
