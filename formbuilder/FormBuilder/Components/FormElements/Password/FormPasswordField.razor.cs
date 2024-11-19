using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Password;

public partial class FormPasswordField : IGenericFormElement<FormPasswordFieldRecord>
{
    [Parameter] public FormPasswordFieldRecord Value { get; set; }
    [Parameter] public FormViewerContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
}
