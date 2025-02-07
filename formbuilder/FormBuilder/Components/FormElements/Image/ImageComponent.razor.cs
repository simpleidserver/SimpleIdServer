using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Image;

public partial class ImageComponent : IGenericFormElement<ImageRecord>
{
    [Parameter] public ImageRecord Value { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
}