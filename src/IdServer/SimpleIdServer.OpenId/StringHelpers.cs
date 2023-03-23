// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.OpenId
{
    internal static class StringHelpers
    {
        public static T ParseValueOrDefault<T>(string? stringValue, Func<string, T> parser, T defaultValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return defaultValue;
            }

            return parser(stringValue);
        }
    }
}
