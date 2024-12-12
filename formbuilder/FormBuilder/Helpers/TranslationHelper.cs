using FormBuilder.Models;

namespace FormBuilder.Helpers;

public interface ITranslationHelper
{
    string Translate(IFormElementRecord record);
    string Translate(IFormElementRecord record, string defaultText);
}

public class TranslationHelper : ITranslationHelper
{
    public string Translate(IFormElementRecord record)
    {
        if (record.Labels == null || !record.Labels.Any()) return string.Empty;
        return record.Labels.First().Translation;
    }

    public string Translate(IFormElementRecord record, string defaultText)
    {
        var result = Translate(record);
        return string.IsNullOrWhiteSpace(result) ? defaultText : result;
    }
}