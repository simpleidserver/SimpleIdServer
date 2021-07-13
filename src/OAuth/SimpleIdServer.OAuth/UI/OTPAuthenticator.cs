// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.OAuth.UI
{
    public class OTPAuthenticator
    {
        protected long GenerateOtp(byte[] key, long counter)
        {
            var data = BitConverter.GetBytes(counter);
            Array.Reverse(data);
            byte[] hashed;
            using (var hmac = new HMACSHA1())
            {
                hmac.Key = key;
                hashed = hmac.ComputeHash(data);
            }

            int offset = hashed[hashed.Length - 1] & 0x0F;
            var otp = (hashed[offset] & 0x7f) << 24
                | (hashed[offset + 1] & 0xff) << 16
                | (hashed[offset + 2] & 0xff) << 8
                | (hashed[offset + 3] & 0xff) % 1000000;
            var truncatedValue = ((int)otp % (int)Math.Pow(10, 6));
            return long.Parse(truncatedValue.ToString().PadLeft(6, '0'));
        }
    }
}
