using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Checkbox;

public partial class FormCheckbox : IGenericFormElement<FormCheckboxRecord>
{
    [Parameter] public FormCheckboxRecord Value { get; set; }
    [Parameter] public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
}
