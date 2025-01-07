using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class FormRecord
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool ActAsStep { get; set; }
    public ObservableCollection<IFormElementRecord> Elements { get; set; } = new ObservableCollection<IFormElementRecord>();
    [JsonIgnore]
    public List<FormStyle> AvailableStyles { get; set; } = new List<FormStyle>();
    [JsonIgnore]
    public FormStyle ActiveStyle
    {
        get
        {
            return AvailableStyles.SingleOrDefault(s => s.IsActive);
        }
    }

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