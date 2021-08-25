// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SimpleIdServer.Saml.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static IEnumerable<KeyValuePair<string, string>> ToEnumerable(this IQueryCollection query)
        {
            var result = new List<KeyValuePair<string, string>>();
            foreach (var record in query)
            {
                result.Add(new KeyValuePair<string, string>(record.Key, record.Value));
            }

            return result;
        }
    }
}
