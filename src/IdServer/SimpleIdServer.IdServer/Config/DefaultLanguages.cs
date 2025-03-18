// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public class DefaultLanguages
{
    public static List<Language> All => new List<Language>
    {
        French,
        English
    };

    public static Language French = LanguageBuilder.Build("fr").AddDescription("French", "en").AddDescription("Français", "fr").Build();
    public static Language English = LanguageBuilder.Build(Language.Default).AddDescription("English", "en").AddDescription("Anglais", "fr").Build();    
}
