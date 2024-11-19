using FormBuilder.Models;
using FormBuilder.Models.Rules;

namespace FormBuilder.Components.FormElements.ListData;

public class ListDataRecord : BaseFormDataRecord
{
    public IRepetitionRule RepetitionRule { get; set; }
    internal IEnumerable<IFormElementRecord> Elements { get; set; }
}