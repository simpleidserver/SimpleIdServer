// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OAuth.Persistence.Parameters
{
    public class SearchClientParameter : BaseSearchParameter
    {
        public SearchClientParameter()
        {
            StartIndex = 0;
            Count = 10;
            OrderBy = "update_datetime";
            Order = SortOrders.DESC;
        }
    }
}
