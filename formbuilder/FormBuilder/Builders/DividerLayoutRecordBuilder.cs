using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Conditions;
using FormBuilder.Models;

namespace FormBuilder.Builders;

public class DividerLayoutRecordBuilder
{
    private readonly DividerLayoutRecord _record;

    internal DividerLayoutRecordBuilder()
    {
        _record = new DividerLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString()
        };
    }

    public DividerLayoutRecordBuilder AddTranslation(string language, string value, IConditionParameter conditionParameter = null)
    {
        _record.Labels.Add(new LabelTranslation(language, value, conditionParameter));
        return this;
    }

    public DividerLayoutRecordBuilder SetContainerClass(string value, string templateName)
    {
        _record.Classes.Add(new HtmlClassRecord
        {
            Element = DividerElementNames.Container,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }

    public DividerLayoutRecordBuilder SetLineClass(string value, string templateName)
    {
        _record.Classes.Add(new HtmlClassRecord
        {
            Element = DividerElementNames.Line,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }

    public DividerLayoutRecordBuilder SetTextClass(string value, string templateName)
    {
        _record.Classes.Add(new HtmlClassRecord
        {
            Element = DividerElementNames.Text,
            TemplateName = templateName,
            Value = value
        });
        return this;
    }

    internal DividerLayoutRecord Build()
    {
        return _record;
    }
}
