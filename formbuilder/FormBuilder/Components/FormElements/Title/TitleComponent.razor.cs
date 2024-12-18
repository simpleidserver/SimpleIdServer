using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Title;

public partial class TitleComponent : IGenericFormElement<TitleRecord>
{
    [Parameter] public TitleRecord Value { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
}
