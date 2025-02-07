using FormBuilder.Components.Drag;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Paragraph;

public partial class ParagraphComponent : IGenericFormElement<ParagraphRecord>
{
    [Parameter] public ParagraphRecord Value { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    public JsonNode InputData
    {
        get
        {
            var linkExecution = Context.GetCurrentStepExecution();
            return linkExecution?.InputData;
        }
    }
}