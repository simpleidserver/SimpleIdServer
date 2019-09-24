// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Jwt.Extensions
{
    public static class DictionaryExtensions
    {
        public static byte[] TryGet(this Dictionary<string, string> dic, string name)
        {
            if (!dic.ContainsKey(name))
            {
                return null;
            }

            return dic[name].Base64DecodeBytes();
        }
    }
}
