using FormBuilder.Models;

namespace FormBuilder.Builders;

public class LabelTranslationBuilder
{
    private readonly List<LabelTranslation> _translations;

    private LabelTranslationBuilder()
    {
        _translations = new List<LabelTranslation>();
    }

    public static LabelTranslationBuilder New()
    {
        return new LabelTranslationBuilder();
    }

    public LabelTranslationBuilder AddTranslation(string language, string translation)
    {
        _translations.Add(new LabelTranslation(language, translation));
        return this;
    }

    public List<LabelTranslation> Build() => _translations;
}
