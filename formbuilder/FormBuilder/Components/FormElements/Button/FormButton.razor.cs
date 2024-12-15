using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Button;

public partial class FormButton : IGenericFormElement<FormButtonRecord>
{
    private bool _isEvtBind = false;
    [Parameter] public FormButtonRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowViewerContext WorkflowContext { get; set; }
    [Parameter] public WorkflowExecutionContext WorkflowExecutionContext { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if(Value != null && !_isEvtBind)
        {
            Value.IsSubmittingChanged += (o, b) =>
            {
                StateHasChanged();
            };
            _isEvtBind = true;
        }
    }
}