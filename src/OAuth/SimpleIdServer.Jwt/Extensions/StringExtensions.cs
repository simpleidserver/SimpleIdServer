// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleIdServer.Jwt.Extensions
{
    public static class StringExtensions
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Base64EncodeBytes(plainTextBytes);
        }

        public static string Base64EncodeBytes(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Split('=')[0]
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            return Encoding.UTF8.GetString(base64EncodedData.Base64DecodeBytes(),
                0,
                base64EncodedData.Base64DecodeBytes().Length);
        }

        public static byte[] Base64DecodeBytes(this string base64EncodedData)
        {
            var s = base64EncodedData
                .Trim()
                .Replace(" ", "+")
                .Replace('-', '+')
                .Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0:
                    return Convert.FromBase64String(s);
                case 2:
                    s += "==";
                    goto case 0;
                case 3:
                    s += "=";
                    goto case 0;
                default:
                    throw new InvalidOperationException("Illegal base64url string!");
            }
        }

        public static IEnumerable<string> GetParts(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return new string[0];
            }

            return str.Split('.');
        }
    }
}
