using FormBuilder.Components.FormElements.Input;
using FormBuilder.Models;
using FormBuilder.Models.Rules;

namespace FormBuilder.Builders;

public class FormInputBuilder
{
    private readonly FormInputFieldRecord _formInput;
    private readonly LabelTranslationBuilder _translationBuilder;

    public FormInputBuilder(string name, FormInputTypes type)
    {
        _translationBuilder = LabelTranslationBuilder.New();
        _formInput = new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            FormType = type
        };
    }

    public static FormInputBuilder New(string name, FormInputTypes type)
    {
        return new FormInputBuilder(name, type);
    }

    public FormInputBuilder AddLabel(string language, string label)
    {
        _translationBuilder.AddTranslation(language, label);
        return this;
    }

    public FormInputBuilder SetContainerHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormInputElementNames.Container, value, templateName);
    }

    public FormInputBuilder SetLabelHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormInputElementNames.Label, value, templateName);
    }

    public FormInputBuilder SetTextBoxHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormInputElementNames.TextBox, value, templateName);
    }

    public FormInputBuilder SetPasswordHtmlClass(string value, string templateName)
    {
        return SetEltHtmlClass(FormInputElementNames.Password, value, templateName);
    }

    public FormInputBuilder SetTransformations(List<ITransformationRule> transformations)
    {
        _formInput.Transformations = transformations;
        return this;
    }

    public FormInputFieldRecord Build()
    {
        _formInput.Labels = _translationBuilder.Build();
        return _formInput;
    }

    private FormInputBuilder SetEltHtmlClass(string elt, string value, string templateName)
    {
        var htmlClass = new HtmlClassRecord
        {
            Element = elt,
            Value = value,
            TemplateName = templateName
        };
        return this;
    }
}
