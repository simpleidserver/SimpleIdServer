namespace FormBuilder.Models;

public class TemplateStyle
{
    public string Id
    {
        get; set;
    }

    public string Value
    {
        get; set;
    }

    public TemplateStyleCategories Category
    {
        get; set;
    }

    public TemplateStyleLanguages Language
    {
        get; set;
    } = TemplateStyleLanguages.Css;
}
