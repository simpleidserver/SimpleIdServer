using System.Collections.ObjectModel;

namespace FormBuilder.Models;

public interface IFormRecordCollection
{
    ObservableCollection<IFormElementRecord> Elements { get; set; }
}
