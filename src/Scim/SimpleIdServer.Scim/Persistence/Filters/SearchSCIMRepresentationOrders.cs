// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Persistence.Filters
{
    public enum SearchSCIMRepresentationOrders
    {
        /// <summary>
        /// Ascending
        /// </summary>
        /// <example>ascending</example>
        Ascending = 0,
        /// <summary>
        /// Descending
        /// </summary>
        /// <example>descending</example>
        Descending = 1
    }
}
