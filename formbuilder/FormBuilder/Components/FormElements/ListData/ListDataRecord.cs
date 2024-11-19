using FormBuilder.Models;
using FormBuilder.Models.Rules;

namespace FormBuilder.Components.FormElements.ListData;

public class ListDataRecord : BaseFormDataRecord
{
    public IRepetitionRule RepetitionRule { get; set; }
    internal List<IFormElementRecord> Elements { get; set; }
}