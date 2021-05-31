// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OAuth.Domains
{
    public class OAuthTranslation : ICloneable
    {
        public OAuthTranslation() { }

        public OAuthTranslation(string key, string value, string language)
        {
            Key = key;
            Value = value;
            Language = language;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string Language { get; set; }
        public string Type { get; set; }

        public object Clone()
        {
            return new OAuthTranslation(Key, Value, Language)
            {
                Type = Type
            };
        }
    }
}