// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OAuth.Persistence.Parameters
{
    public class SearchTokenParameter : BaseSearchParameter
    {
        public SearchTokenParameter()
        {
            StartIndex = 0;
            Count = 10;
            OrderBy = "create_datetime";
            Order = SortOrders.DESC;
        }

        public string TokenType { get; set; }
        public string ClientId { get; set; }
        public string AuthorizationCode { get; set; }
    }
}
