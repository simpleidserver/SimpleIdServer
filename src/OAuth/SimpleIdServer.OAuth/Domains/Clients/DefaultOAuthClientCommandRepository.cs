// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Domains.Clients
{
    public class DefaultOAuthClientCommandRepository : InMemoryCommandRepository<OAuthClient>, IOAuthClientCommandRepository
    {
        public DefaultOAuthClientCommandRepository(List<OAuthClient> clients) : base(clients)
        {
        }
    }
}