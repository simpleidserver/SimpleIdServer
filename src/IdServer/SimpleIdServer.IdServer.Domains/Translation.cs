// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains
{
    public class Translation : ICloneable
    {
        public Translation() { }

        public Translation(string key, string value, string language)
        {
            Key = key;
            Value = value;
            Language = language;
        }

        public string Key { get; set; } = null!;
        public string? Value { get; set; } = null!;
        public string? Language { get; set; } = null;

        public object Clone()
        {
            return new Translation(Key, Value, Language);
        }
    }
}
