using FormBuilder.Models;

namespace FormBuilder.Components.FormElements.Divider;

public class DividerLayoutRecord : BaseFormLayoutRecord
{
    public override string Type => DividerLayoutDefinition.TYPE;

    public override int NbElements => 0;
}
