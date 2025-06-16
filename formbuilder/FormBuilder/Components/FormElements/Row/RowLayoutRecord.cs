using FormBuilder.Models;

namespace FormBuilder.Components.FormElements.Row
{
    public class RowLayoutRecord : BaseFormLayoutRecord
    {
        public override string Type => RowLayoutDefinition.TYPE;

        public int NbColumns
        {
            get; set;
        } = 2;

        public override int NbElements => NbColumns;
    }
}
