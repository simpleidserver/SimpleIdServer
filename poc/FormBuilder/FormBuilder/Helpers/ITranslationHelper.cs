using FormBuilder.Models;

namespace FormBuilder.Helpers;

public interface ITranslationHelper
{
    string Translate(IFormElementRecord record);
}

public class TranslationHelper : ITranslationHelper
{
    public string Translate(IFormElementRecord record)
    {
        if (record.Labels == null || !record.Labels.Any()) return string.Empty;
        return record.Labels.First().Translation;
    }
}