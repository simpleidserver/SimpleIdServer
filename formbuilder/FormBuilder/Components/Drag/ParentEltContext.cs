using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder.Components.Drag;

public class ParentEltContext
{
    public ParentEltContext(bool isParentBlocked, int parentLevel, ObservableCollection<IFormElementRecord> elements, int index)
    {
        IsParentBlocked = isParentBlocked;
        ParentLevel = parentLevel;
        Elements = elements;
        Index = index;
    }

    public bool IsParentBlocked { get; set; }
    public int ParentLevel { get; set; }
    public ObservableCollection<IFormElementRecord> Elements { get; set; }
    public int Index { get; set; }
}
