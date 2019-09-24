// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Extensions
{
    public static class HMACExtensions
    {
        public static void Import(this HMAC hmac, Dictionary<string, string> key)
        {
            var k = Convert.FromBase64String(key["k"]);
            hmac.Key = k;
        }

        public static Dictionary<string, string> ExportKey(this HMAC hmac)
        {
            return new Dictionary<string, string>
            {
                { "k", Convert.ToBase64String(hmac.Key) }
            };
        }
    }
}