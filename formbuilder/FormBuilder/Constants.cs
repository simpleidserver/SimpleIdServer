using FormBuilder.Components.FormElements.Paragraph;
using FormBuilder.Models;
using System.Collections.ObjectModel;

namespace FormBuilder;

public static class Constants
{
    public static FormRecord EmptyStep => new FormRecord
    {
        Id = "emptyStep",
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