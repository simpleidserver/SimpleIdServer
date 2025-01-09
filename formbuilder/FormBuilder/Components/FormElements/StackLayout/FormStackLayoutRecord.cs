using FormBuilder.Components.FormElements.Button;
using FormBuilder.Models;
using System.Text.Json.Serialization;

namespace FormBuilder.Components.FormElements.StackLayout;

public class FormStackLayoutRecord : BaseFormLayoutRecord
{
    public override string Type => FormStackLayoutDefinition.TYPE;
    public bool IsFormEnabled { get; set; } = false;
    public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    [JsonIgnore]
    public bool IsSubmitting { get; set; }
    public FormTypes FormType { get; set; } = FormTypes.BLAZOR;
    public bool IsNotVisible { get; set; }

    public void Submit()
    {
        IsSubmitting = true;
        var submitBtn = GetSubmitBtn();
        if(submitBtn != null) submitBtn.IsSubmitting = true;
    }

    public void FinishSubmit()
    {
        IsSubmitting = false;
        var submitBtn = GetSubmitBtn();
        if (submitBtn != null) submitBtn.IsSubmitting = false;
    }

    private FormButtonRecord GetSubmitBtn()
        => Elements.SingleOrDefault(e => e is FormButtonRecord) as FormButtonRecord;
}

public enum FormTypes
{
    BLAZOR = 0,
    HTML = 1
}