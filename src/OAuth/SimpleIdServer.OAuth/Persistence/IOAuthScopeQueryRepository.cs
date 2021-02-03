// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface IOAuthScopeQueryRepository
    {
        Task<List<OAuthScope>> GetAllOAuthScopes();
        Task<List<OAuthScope>> GetAllOAuthScopesExposed();
        Task<List<OAuthScope>> FindOAuthScopesByNames(IEnumerable<string> names, CancellationToken token);
    }
}