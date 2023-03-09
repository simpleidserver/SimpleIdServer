// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SimpleIdServer.IdServer.Builders
{
    public class UMAResourceBuilder
    {
        private readonly UMAResource _umaResource;

        private UMAResourceBuilder(string id, IEnumerable<string> scopes)
        {
            _umaResource = new UMAResource
            {
                Id = id,
                Scopes = scopes.ToList(),
                Realm = Constants.DefaultRealm
            };
        }

        public static UMAResourceBuilder Create(string id, params string[] scopes)
        {
            var result = new UMAResourceBuilder(id, scopes);
            return result;
        }

        public UMAResourceBuilder SetName(string name, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            _umaResource.Translations.Add(new Translation
            {
                Key = "name",
                Value = name,
                Language = language
            });
            return this;
        }

        public UMAResourceBuilder SetDescription(string description, string language = null)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            _umaResource.Translations.Add(new Translation
            {
                Key = "description",
                Value = description,
                Language = language
            });
            return this;
        }

        public UMAResource Build() => _umaResource;
    }
}
