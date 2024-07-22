// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IAuthenticationSchemeProviderRepository
{
    void Add(AuthenticationSchemeProvider idProvider);
    Task<List<AuthenticationSchemeProvider>> GetAll(string realm, CancellationToken cancellationToken);
    Task<AuthenticationSchemeProvider> Get(string realm, string name, CancellationToken cancellationToken);
    Task<SearchResult<AuthenticationSchemeProvider>> Search(string realm, SearchRequest request, CancellationToken cancellationToken);
    void Remove(AuthenticationSchemeProvider idProvider);
    void Update(AuthenticationSchemeProvider idProvider);
}