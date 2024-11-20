using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder.Components.Drag;

public class ParentEltContext
{
    public ParentEltContext(ObservableCollection<IFormElementRecord> elements, int index)
    {
        Elements = elements;
        Index = index;
    }

    public ObservableCollection<IFormElementRecord> Elements { get; set; }
    public int Index { get; set; }
}
