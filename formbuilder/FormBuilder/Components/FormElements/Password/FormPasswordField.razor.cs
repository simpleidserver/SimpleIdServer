using FormBuilder.Components.Drag;
using FormBuilder.Helpers;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.Password;

public partial class FormPasswordField : IGenericFormElement<FormPasswordFieldRecord>
{
    private bool isPasswordVisible = false;
    [Parameter] public FormPasswordFieldRecord Value { get; set; }
    [Parameter] public WorkflowContext Context { get; set; }
    [Parameter] public bool IsEditModeEnabled { get; set; }
    [Parameter] public ParentEltContext ParentContext { get; set; }
    [Inject] public IHtmlClassResolver HtmlClassResolver { get; set; }

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
            var result = "rz-form-field rz-variant-outlined rz-floating-label";
            var resolvedClass = HtmlClassResolver.Resolve(Value, FormPasswordElementNames.Container, Context);
            if(!string.IsNullOrWhiteSpace(resolvedClass))
            {
                result += " " + resolvedClass;
            }

            return result;
        }
    }

    public string LabelClass
    {
        get
        {
            return HtmlClassResolver.Resolve(Value, FormPasswordElementNames.Label, Context);
        }
    }

    public string PasswordClass
    {
        get
        {
            return HtmlClassResolver.Resolve(Value, FormPasswordElementNames.Password, Context);
        }
    }

    public string ViewPasswordContainerClass
    {
        get
        {
            return HtmlClassResolver.Resolve(Value, FormPasswordElementNames.ViewPasswordContainer, Context);
        }
    }

    public string ViewPasswordBtnClass
    {
        get
        {
            return HtmlClassResolver.Resolve(Value, FormPasswordElementNames.ViewPasswordBtn, Context);
        }
    }

    private void TogglePasswordVisibility()
    {
        isPasswordVisible = !isPasswordVisible;
    }
}
