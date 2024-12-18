using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Paragraph;

public partial class ParagraphComponent : IGenericFormElement<ParagraphRecord>
{
    [Parameter] public ParagraphRecord Value { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public bool IsInteractableElementEnabled { get; set; }
}