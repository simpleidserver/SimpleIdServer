// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers.Models;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders;

public class LanguageBuilder
{
    private readonly Language _language;

    private LanguageBuilder(string language)
    {
        _language = new Language
        {
            Code = language,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
    }

    public static LanguageBuilder Build(string language)
        => new LanguageBuilder(language);

    public LanguageBuilder AddDescription(string description, string? language = null)
    {
        language = language ?? _language.Code;
        _language.Descriptions = _language.Descriptions.Where(d => d.Language != language).ToList();
        _language.Descriptions.Add(new Translation
        {
            Language = language,
            Value = description,
            Key = _language.TranslationKey
        });
        return this;
    }

    public Language Build()
        => _language;
}
