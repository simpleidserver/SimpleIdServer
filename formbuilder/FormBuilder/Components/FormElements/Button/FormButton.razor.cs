using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Button;

public partial class FormButton : IGenericFormElement<FormButtonRecord>
{
    [Parameter] public FormButtonRecord Value { get; set; }
    [Parameter] public AntiforgeryTokenRecord AntiforgeryToken { get; set; }
}