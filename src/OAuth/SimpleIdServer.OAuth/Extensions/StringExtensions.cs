// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class StringExtensions
    {
        public static ICollection<string> ToScopes(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? new List<string>() : str.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }

        public static string Join(this IEnumerable<string> arr, string separator = ",")
        {
            return string.Join(separator, arr);
        }

        public static string ExtractAuthorizationValue(this string str, IEnumerable<string> authenticationSchemes)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            var splitted = str.Split(' ');
            if (splitted.Count() != 2)
            {
                return null;
            }

            var first = splitted.First();
            if (!authenticationSchemes.Contains(first))
            {
                return null;
            }

            return splitted.Last();
        }
    }
}
