using System.Collections.ObjectModel;

namespace FormBuilder.Models;

public class FormRecord
{
    public string Name { get; set; }
    public ObservableCollection<IFormElementRecord> Elements { get; set; } = new ObservableCollection<IFormElementRecord>();

    public IFormElementRecord GetChild(string id)
    {
        var result = Elements.SingleOrDefault(e => e.Id == id);
        if (result != null) return result;
        foreach(var elt in Elements)
        {
            var child = elt.GetChild(id);
            if(child != null) return child;
        }

        return null;
    }

    public bool HasChild(string id)
        => GetChild(id) != null;
}