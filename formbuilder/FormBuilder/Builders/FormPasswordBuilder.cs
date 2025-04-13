using FormBuilder.Components.FormElements.Password;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class FormPasswordBuilder
{
    private readonly FormPasswordFieldRecord _pwdRecord;

    public FormPasswordBuilder(string name)
    {
        _pwdRecord = new FormPasswordFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name
        };
    }

    public FormPasswordBuilder AddLabel(string language, string label)
    {
        _pwdRecord.Labels.Add(new LabelTranslation(language, label, null));
        return this;
    }

    public FormPasswordBuilder SetContainerHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormPasswordElementNames.Container, value, templateName);
    }

    public FormPasswordBuilder SetLabelHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormPasswordElementNames.Label, value, templateName);
    }

    public FormPasswordBuilder SetPasswordHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormPasswordElementNames.Password, value, templateName);
    }

    internal FormPasswordFieldRecord Build()
    {
        return _pwdRecord;
    }

    private FormPasswordBuilder SetEltHtmlClass(string elt, string value, string templateName)
    {
        var htmlClass = new HtmlClassRecord
        {
            Element = elt,
            Value = value,
            TemplateName = templateName
        };
        _pwdRecord.Classes.Add(htmlClass);
        return this;
    }
}
