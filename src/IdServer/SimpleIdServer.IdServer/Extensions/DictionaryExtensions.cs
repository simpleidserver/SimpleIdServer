// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static void AddOrReplace(this Dictionary<string, object> dictionary, string key, object value)
        {
            if(dictionary.ContainsKey(key)) dictionary[key] = value;
            else dictionary.Add(key, value);
        }
    }
}
