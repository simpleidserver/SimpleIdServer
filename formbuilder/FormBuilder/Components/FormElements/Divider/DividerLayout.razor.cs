using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Divider;

public partial class DividerLayout : IGenericFormElement<DividerLayoutRecord>
{
    [Parameter] public DividerLayoutRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Inject] private IHtmlClassResolver htmlClassResolver { get; set; }
    public JsonNode InputData
    {
        get
        {
            var linkExecution = Context.GetCurrentStepExecution();
            return linkExecution?.InputData;
        }
    }

    public string ContainerClass
    {
        get
        {
            return htmlClassResolver.Resolve(Value, DividerElementNames.Container, Context);
        }
    }

    public string TextClass
    {
        get
        {
            
            return htmlClassResolver.Resolve(Value, DividerElementNames.Text, Context);
        }
    }
}