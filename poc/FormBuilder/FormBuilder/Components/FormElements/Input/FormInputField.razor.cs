using Microsoft.AspNetCore.Components;

namespace FormBuilder.Components.FormElements.Input
{
    public partial class FormInputField : IGenericFormElement<FormInputFieldRecord>
    {
        [Parameter] public FormInputFieldRecord Value { get; set; }
    }
}
