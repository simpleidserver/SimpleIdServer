// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Persistence.Results
{
    public class SearchResult<T>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public int TotalLength { get; set; }
        public ICollection<T> Content { get; set; }
    }
}