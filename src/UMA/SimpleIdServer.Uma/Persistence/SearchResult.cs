// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Uma.Persistence
{
    public class SearchResult<T>
    {
        public int TotalResults { get; set; }
        public ICollection<T> Records { get; set; }
    }
}
