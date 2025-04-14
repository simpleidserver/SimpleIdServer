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

    internal FormPasswordFieldRecord Build()
    {
        return _pwdRecord;
    }
}
