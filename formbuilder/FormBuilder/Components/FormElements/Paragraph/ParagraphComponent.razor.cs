using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Paragraph;

public partial class ParagraphComponent : IGenericFormElement<ParagraphRecord>
{
    [Parameter] public ParagraphRecord Value { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Inject] private IHtmlClassResolver htmlClassResolver { get; set; }
    public string ContainerClass
    {
        get
        {
            return htmlClassResolver.Resolve(Value, ParagraphElementNames.Container, Context);
        }
    }

    public JsonNode InputData
    {
        get
        {
            var linkExecution = Context.GetCurrentStepExecution();
            return linkExecution?.InputData;
        }
    }
}