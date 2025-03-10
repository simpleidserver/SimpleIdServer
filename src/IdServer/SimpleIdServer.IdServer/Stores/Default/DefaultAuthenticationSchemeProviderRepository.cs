// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultAuthenticationSchemeProviderRepository : IAuthenticationSchemeProviderRepository
{
    private readonly List<AuthenticationSchemeProvider> _authenticationSchemeProviders;

    public DefaultAuthenticationSchemeProviderRepository(List<AuthenticationSchemeProvider> authenticationSchemeProviders)
    {
        _authenticationSchemeProviders = authenticationSchemeProviders;
    }

    public void Add(AuthenticationSchemeProvider idProvider) => _authenticationSchemeProviders.Add(idProvider);

    public Task<List<AuthenticationSchemeProvider>> GetAll(string realm, CancellationToken cancellationToken)
    {
        return Task.FromResult(_authenticationSchemeProviders
            .Where(c => c.Realms.Any(r => r.Name == realm))
            .ToList());
    }

    public Task<AuthenticationSchemeProvider> Get(string realm, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_authenticationSchemeProviders
            .Where(p => p.Realms.Any(r => r.Name == realm))
            .SingleOrDefault(p => p.Name == name));
    }

    public async Task<SearchResult<AuthenticationSchemeProvider>> Search(string realm, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = _authenticationSchemeProviders
            .Where(p => p.Realms.Any(r => r.Name == realm))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);

        var nb = query.Count();
        var idProviders = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        return await Task.FromResult(new SearchResult<AuthenticationSchemeProvider>
        {
            Count = nb,
            Content = idProviders
        });
    }

    public void Remove(AuthenticationSchemeProvider idProvider) => _authenticationSchemeProviders.Remove(idProvider);

    public void Update(AuthenticationSchemeProvider idProvider)
    {
        var index = _authenticationSchemeProviders.FindIndex(p => p.Id == idProvider.Id);
        if (index != -1)
        {
            _authenticationSchemeProviders[index] = idProvider;
        }
    }
}
