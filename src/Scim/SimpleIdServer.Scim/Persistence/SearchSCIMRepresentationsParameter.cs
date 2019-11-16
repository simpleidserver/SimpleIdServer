// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;

namespace SimpleIdServer.Scim.Persistence
{
    public class SearchSCIMRepresentationsParameter
    {
        public SearchSCIMRepresentationsParameter(string resourceType, int startIndex, int count, string sortBy, SearchSCIMRepresentationOrders? sortOrder = null, SCIMExpression filter = null)
        {
            ResourceType = resourceType;
            StartIndex = startIndex;
            Count = count;
            SortBy = sortBy;
            SortOrder = sortOrder;
            Filter = filter;
        }

        public string ResourceType { get; set; }
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public string SortBy { get; set; }
        public SearchSCIMRepresentationOrders? SortOrder { get; set; }
        public SCIMExpression Filter { get; set; } 
    }
}
