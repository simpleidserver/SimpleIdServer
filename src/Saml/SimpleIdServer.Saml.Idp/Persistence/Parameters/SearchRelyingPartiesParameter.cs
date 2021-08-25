// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Persistence;

namespace SimpleIdServer.Saml.Idp.Persistence.Parameters
{
    public class SearchRelyingPartiesParameter : BaseSearchParameter
    {
        public SearchRelyingPartiesParameter()
        {
            StartIndex = 0;
            Count = 10;
            OrderBy = "update_datetime";
            Order = SortOrders.DESC;
        }
    }
}
