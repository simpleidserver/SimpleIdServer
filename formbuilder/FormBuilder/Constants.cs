using FormBuilder.Components.FormElements.Paragraph;
using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder;

public static class Constants
{
    public static FormRecord EmptyStep => new FormRecord
    {
        Id = "e58c55fc-cb29-44b6-98b6-87f796147604",
        Name = "Empty",
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new ParagraphRecord
            {
                Id = Guid.NewGuid().ToString(),
                Labels = new List<LabelTranslation>
                {
                    new LabelTranslation { Language = "en", Translation = "Empty state" }
                }
            }
        }
    };
}