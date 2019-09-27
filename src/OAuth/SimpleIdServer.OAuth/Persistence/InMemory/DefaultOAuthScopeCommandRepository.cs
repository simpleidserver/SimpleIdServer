// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultOAuthScopeCommandRepository : InMemoryCommandRepository<OAuthScope>, IOAuthScopeCommandRepository
    {
        public DefaultOAuthScopeCommandRepository(List<OAuthScope> scopes) : base(scopes)
        {
        }
    }
}