// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Api
{
    public interface ITranslatableRequest
    {
        public ICollection<TranslationRequest> Translations { get; set; }
    }

    public class TranslationRequest
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
