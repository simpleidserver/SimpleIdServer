using FormBuilder.Components.FormElements.StackLayout;

namespace FormBuilder.Builders;

public class FormStackLayoutRecordBuilder : BaseFormRecordCollectionBuilder<FormStackLayoutRecordBuilder, FormStackLayoutRecord>
{
    internal FormStackLayoutRecordBuilder(string id = null, string correlationId = null) : base(new FormStackLayoutRecord
    {
        Id = id ?? Guid.NewGuid().ToString(),
        CorrelationId = correlationId ?? Guid.NewGuid().ToString(),
        FormType = FormTypes.BLAZOR
    })
    {
    }

    public FormStackLayoutRecordBuilder SetType(FormTypes formType)
    {
        Record.FormType = formType;
        return this;
    }

    public FormStackLayoutRecordBuilder EnableForm()
    {
        Record.IsFormEnabled = true;
        return this;
    }
}
