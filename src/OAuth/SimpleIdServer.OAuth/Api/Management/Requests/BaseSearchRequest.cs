// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OAuth.Api.Management.Requests
{
    public class BaseSearchRequest
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string OrderBy { get; set; }
        public SortOrders Order { get; set; }
    }
}
