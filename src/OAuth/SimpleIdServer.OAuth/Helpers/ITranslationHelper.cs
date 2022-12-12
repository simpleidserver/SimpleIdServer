// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Helpers
{
    public interface ITranslationHelper
    {
        string Translate(ICollection<Translation> translations, string defaultValue = null);
    }
}
