// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Cryptography;

namespace SimpleIdServer.IdServer.Domains
{
    public class KeyGeneration
    {
        public static byte[] GenerateRandomKey(int length)
        {
            var key = new byte[length];
            using (var rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(key);
                return key;
            }
        }
    }
}
