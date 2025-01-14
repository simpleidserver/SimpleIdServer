using FormBuilder.Conditions;
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

    public LabelTranslationBuilder AddTranslation(string language, string translation, IConditionParameter conditionParameter = null)
    {
        _translations.Add(new LabelTranslation(language, translation, conditionParameter));
        return this;
    }

    public List<LabelTranslation> Build() => _translations;
}
