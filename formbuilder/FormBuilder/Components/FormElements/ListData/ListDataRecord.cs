using FormBuilder.Models;
using FormBuilder.Models.Rules;
using System.Collections.ObjectModel;

namespace FormBuilder.Components.FormElements.ListData;

public class ListDataRecord : BaseFormDataRecord
{
    public IRepetitionRule RepetitionRule { get; set; }
    internal ObservableCollection<(IFormElementRecord, WorkflowExecutionContext)> Elements { get; set; }
}