using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Password;

public partial class FormPasswordField : IGenericFormElement<FormPasswordFieldRecord>
{
    [Parameter] public FormPasswordFieldRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    public JsonNode InputData
    {
        get
        {
            var linkExecution = Context.GetCurrentStepExecution();
            return linkExecution?.InputData;
        }
    }
}
