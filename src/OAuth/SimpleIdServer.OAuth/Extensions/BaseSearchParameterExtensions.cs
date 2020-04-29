// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Persistence.Parameters;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Extensions
{
    public static class BaseSearchParameterExtensions
    {
        public static void ExtractSearchParameter(this BaseSearchParameter parameter, IEnumerable<KeyValuePair<string, string>> query)
        {
            int startIndex, count;
            string orderBy;
            SortOrders findOrder;
            if (query.TryGet("start_index", out startIndex))
            {
                parameter.StartIndex = startIndex;
            }

            if (query.TryGet("count", out count))
            {
                parameter.Count = count;
            }

            if (query.TryGet("order_by", out orderBy))
            {
                parameter.OrderBy = orderBy;
            }

            if (query.TryGet("order", out findOrder))
            {
                parameter.Order = findOrder;
            }
        }
    }
}
