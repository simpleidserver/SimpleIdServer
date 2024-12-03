using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Input;

public partial class FormInputField : IGenericFormElement<FormInputFieldRecord>
{
    [Parameter] public FormInputFieldRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
    }
}
