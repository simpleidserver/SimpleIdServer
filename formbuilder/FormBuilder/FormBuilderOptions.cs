namespace FormBuilder;

public class FormBuilderOptions
{
    public List<Language> SupportedLanguages { get; set; } = new List<Language>()
    {
        new Language { Code = "en", DisplayName = "English" },
        new Language { Code = "fr", DisplayName = "French" }
    };

    public string AntiforgeryCookieName { get; set; } = "XSFR-TOKEN";
}
