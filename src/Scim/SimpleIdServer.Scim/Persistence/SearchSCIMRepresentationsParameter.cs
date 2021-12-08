// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence
{
    public class SearchSCIMRepresentationsParameter
    {
        public SearchSCIMRepresentationsParameter(string resourceType, int startIndex, int count, SCIMExpression sortBy, SearchSCIMRepresentationOrders? sortOrder = null, SCIMExpression filter = null, IEnumerable<SCIMAttributeExpression> includedAttributes = null, IEnumerable<SCIMAttributeExpression> excludedAttributes = null)
        {
            ResourceType = resourceType;
            StartIndex = startIndex;
            Count = count;
            SortBy = sortBy;
            SortOrder = sortOrder;
            Filter = filter;
            IncludedAttributes = includedAttributes;
            ExcludedAttributes = excludedAttributes;
        }

        public IEnumerable<SCIMAttributeExpression> IncludedAttributes { get; set; }
        public IEnumerable<SCIMAttributeExpression> ExcludedAttributes { get; set; }
        public string ResourceType { get; set; }
        public int StartIndex { get; set; }
        public int Count { get; set; }
        public SCIMExpression SortBy { get; set; }
        public SearchSCIMRepresentationOrders? SortOrder { get; set; }
        public SCIMExpression Filter { get; set; } 
    }
}
