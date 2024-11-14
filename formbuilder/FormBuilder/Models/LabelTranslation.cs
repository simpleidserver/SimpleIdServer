namespace FormBuilder.Models;

public class LabelTranslation
{
    public LabelTranslation()
    {
        
    }

    public LabelTranslation(string language, string translation)
    {
        Language = language;
        Translation = translation;
    }

    public string Translation { get; set; }
    public string Language { get; set; }
}
