using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class FormRecord : BaseVersionRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Category { get; set; }
    public string? Realm { get; set; }
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

    public void Update(List<IFormElementRecord> elements, DateTime dateTime)
    {
        UpdateDateTime = dateTime;
        Elements = new ObservableCollection<IFormElementRecord>(elements);
    }

    public IFormElementRecord GetChild(string id)
    {
        var result = Elements.SingleOrDefault(e => e.Id == id);
        if (result != null) return result;
        foreach (var elt in Elements)
        {
            var child = elt.GetChild(id);
            if (child != null) return child;
        }

        return null;
    }

    public IFormElementRecord GetChildByCorrelationId(string correlationId)
    {
        var result = Elements.SingleOrDefault(e => e.CorrelationId == correlationId);
        if (result != null) return result;
        foreach(var elt in Elements)
        {
            var child = elt.GetChildByCorrelationId(correlationId);
            if (child != null) return child;
        }

        return null;
    }

    public override BaseVersionRecord NewDraft(DateTime currentDateTime)
    {
        var clonedElements = JsonSerializer.Deserialize<List<IFormElementRecord>>(JsonSerializer.Serialize(Elements));
        var availableStyles = AvailableStyles?.Select(s => (FormStyle)s.Clone()).ToList() ?? new List<FormStyle>();
        clonedElements.ForEach(c => c.Id = Guid.NewGuid().ToString());
        availableStyles.ForEach(c => c.Id = Guid.NewGuid().ToString());
        return new FormRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = CorrelationId,
            Name = Name,
            Category = Category,
            ActAsStep = ActAsStep,
            AvailableStyles = availableStyles, 
            UpdateDateTime = currentDateTime,
            Status = RecordVersionStatus.Draft,
            Realm = Realm,
            VersionNumber = VersionNumber + 1,
            Elements = new ObservableCollection<IFormElementRecord>(clonedElements)
        };
    }

    public bool HasChild(string id)
        => GetChild(id) != null;
}