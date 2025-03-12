// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Helpers.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Configuration.Models;

public class ConfigurationDefinitionRecordValue : ITranslatable
{
    private const string NAME_KEY = "name";

    public string Id { get; set; } = null!;

    public string Name
    {
        get
        {
            return GetTranslation(Names);
        }
    }

    public List<Translation> Names
    {
        get
        {
            return Translations.Where(t => t.Key == NAME_KEY).ToList();
        }
    }

    public string Value { get; set; } = null!;

    public void SetName(string value, string language) => SetTranslation(NAME_KEY, language, value);
}
