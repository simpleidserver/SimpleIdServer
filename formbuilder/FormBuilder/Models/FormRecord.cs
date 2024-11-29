using System.Collections.ObjectModel;

namespace FormBuilder.Models;

public class FormRecord
{
    public string Name { get; set; }
    public ObservableCollection<IFormElementRecord> Elements { get; set; } = new ObservableCollection<IFormElementRecord>();
}