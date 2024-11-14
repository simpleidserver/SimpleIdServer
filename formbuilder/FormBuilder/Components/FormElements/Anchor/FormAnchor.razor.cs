using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Anchor;

public partial class FormAnchor : IGenericFormElement<FormAnchorRecord>
{
    [Parameter] public FormAnchorRecord Value { get; set; }
}