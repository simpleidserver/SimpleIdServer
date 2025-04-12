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
            var result = "divider-container";
            var resolvedClass = htmlClassResolver.Resolve(Value, DividerElementNames.Container, Context);
            if (!string.IsNullOrWhiteSpace(resolvedClass))
            {
                result += $" {resolvedClass}";
            }

            return result;
        }
    }

    public string LineClass
    {
        get
        {
            return htmlClassResolver.Resolve(Value, DividerElementNames.Line, Context);
        }
    }

    public string TextClass
    {
        get
        {
            var result = "text";
            var resolvedClass = htmlClassResolver.Resolve(Value, DividerElementNames.Text, Context);
            if (!string.IsNullOrWhiteSpace(resolvedClass))
            {
                result += $" {resolvedClass}";
            }

            return result;
        }
    }
}