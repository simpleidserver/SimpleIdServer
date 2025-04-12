using FormBuilder.Models;

namespace FormBuilder;

public class RadzenTemplate
{
    public const string Name = "Radzen";

    public static List<FormStyle> BuildDefault()
    {
        return new List<FormStyle>
        {
            new FormStyle
            {
                Category = FormStyleCategories.Lib,
                Value = "/_content/Radzen.Blazor/css/default.css",
                Id = Guid.NewGuid().ToString(),
                IsActive = true,
                TemplateName = Name
            }
        };
    }
}
