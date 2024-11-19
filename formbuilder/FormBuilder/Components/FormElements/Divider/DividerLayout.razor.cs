using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Divider;

public partial class DividerLayout : IGenericFormElement<DividerLayoutRecord>
{
    [Parameter] public DividerLayoutRecord Value { get; set; }
    [Parameter] public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
}