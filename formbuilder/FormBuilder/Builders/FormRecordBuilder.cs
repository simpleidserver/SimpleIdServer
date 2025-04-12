using FormBuilder.Components.Form;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class FormRecordBuilder : BaseFormRecordCollectionBuilder<FormRecordBuilder, FormRecord>
{
    private FormRecordBuilder(string name, string correlationId, string realm, bool actAsStep) : base(new FormRecord
    {
        Id = Guid.NewGuid().ToString(),
        Name = name,
        CorrelationId = correlationId,
        Realm = realm,
        VersionNumber = 0,
        Status = RecordVersionStatus.Published,
        ActAsStep = actAsStep
    })
    {
    }

    public static FormRecordBuilder New(string name, string correlationId, string realm, bool actAsStep)
    {
        return new FormRecordBuilder(name, correlationId, realm, actAsStep);
    }

    public FormRecordBuilder SetContainerClass(string value, string templateName)
    {
        Record.Classes.Add(new HtmlClassRecord
        {
            Element = FormElementNames.Container,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }

    public FormRecordBuilder SetContentClass(string value, string templateName)
    {
        Record.Classes.Add(new HtmlClassRecord
        {
            Element = FormElementNames.Content,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }

    public FormRecordBuilder SetFormContainerClass(string value, string templateName)
    {
        Record.Classes.Add(new HtmlClassRecord
        {
            Element = FormElementNames.FormContainer,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }

    public FormRecordBuilder SetFormContentClass(string value, string templateName)
    {
        Record.Classes.Add(new HtmlClassRecord
        {
            Element = FormElementNames.FormContent,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }
}
