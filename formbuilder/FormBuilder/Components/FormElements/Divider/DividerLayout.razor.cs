using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Divider;

public partial class DividerLayout : IGenericFormElement<DividerLayoutRecord>
{
    [Parameter] public DividerLayoutRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
}