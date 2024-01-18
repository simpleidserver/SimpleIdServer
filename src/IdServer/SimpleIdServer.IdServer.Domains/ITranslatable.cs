// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class ITranslatable
    {
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();

        protected void SetTranslation(string key, string language, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var translation = Translations.SingleOrDefault(t => t.Key == key && t.Language == language);
            if (translation == null)
                Translations.Add(new Translation { Key = key, Language = language, Value = value });
            else
                translation.Value = value;
        }

        protected string? GetTranslation(ICollection<Translation> translations)
        {
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            var result = translations.SingleOrDefault(t => t.Language == language);
            return result?.Value;
        }
    }
}
