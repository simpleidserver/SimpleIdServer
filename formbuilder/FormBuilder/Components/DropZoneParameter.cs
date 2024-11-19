using FormBuilder.Models;

namespace FormBuilder.Components;

public class DropZoneParameter
{
    public DropZoneParameter(List<IFormElementRecord> elements, int index)
    {
        Elements = elements;
        Index = index;
    }

    public List<IFormElementRecord> Elements { get; set; }
    public int Index { get; set; }
}
