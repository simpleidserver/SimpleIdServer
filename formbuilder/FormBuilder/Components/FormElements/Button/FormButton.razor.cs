using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Button;

public partial class FormButton : IGenericFormElement<FormButtonRecord>
{
    private bool _isEvtBind = false;
    [Parameter] public FormButtonRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Inject] private IHtmlClassResolver HtmlClassResolver { get; set; }

    public string BtnClass
    {
        get
        {
            return HtmlClassResolver.Resolve(Value, FormButtonElementNames.Btn, Context);
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

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if(Value != null && !_isEvtBind)
        {
            Value.IsSubmittingChanged += (o, b) =>
            {
                StateHasChanged();
            };
            _isEvtBind = true;
        }
    }
}