# Add language

## Add a New Language to the Identity Server Website

By default, the Identity Server website supports only the `English` language.

However, it is still possible to incorporate support for a new language by making modifications in the codebase of the project.

In the remainder of this article, we will explain the manual steps to enable support for a new language.

1. **Register the language in the Identity Server** : Open the Identity Server solution and edit the `IdServerConfiguration.cs` file. Add a new language to the `Languages` property and specify its English translation. For example, to support the `French` language, you can have the following configuration :

```
public static ICollection<Language> Languages => new List<Language>
{
    LanguageBuilder.Build(Language.Default).AddDescription("English", "en").AddDescription("Anglais", "fr").Build(),
    LanguageBuilder.Build("fr").AddDescription("Français", "fr").AddDescription("French", "en").Build()
};
```

2. **Register the Language in the Identity Server website** : Open the Identity Server website solution and edit the `Program.cs` file. Modify the callback function of the `UseRequestLocalization` method and add the language code. For instance, to support the `French` language, use the following configuration:

```
app.UseRequestLocalization(e =>
{
    e.SetDefaultCulture("en");
    e.AddSupportedCultures("en", "fr");
    e.AddSupportedUICultures("en", "fr");
});
```

3. **Resource file** : Each SimpleIdServer project contain a `Global.resx` file with all the `English` translations. To update the translation of the Identity Server website, create a `Global.<two letter language code>.resx` file in the `SimpleIdServer.IdServer.Website\Resources` folder.

If you feel comfortable, you can easily contribute to the project by adding a new language :)