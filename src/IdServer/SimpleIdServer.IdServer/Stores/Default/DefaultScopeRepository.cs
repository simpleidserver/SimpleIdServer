// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Api.Scopes;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultScopeRepository : IScopeRepository
{
    private readonly List<Scope> _scopes;

    public DefaultScopeRepository(List<Scope> scopes)
    {
        _scopes = scopes;
    }

    public Task<List<Scope>> Get(List<string> ids, CancellationToken cancellationToken)
    {
        var result = _scopes
            .Where(s => ids.Contains(s.Id))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<Scope> Get(string realm, string id, CancellationToken cancellationToken)
    {
        var scope = _scopes
            .FirstOrDefault(s => s.Id == id && s.Realms.Any(r => r.Name == realm));
        return Task.FromResult(scope);
    }

    public Task<Scope> GetByName(string realm, string name, CancellationToken cancellationToken)
    {
        var scope = _scopes
            .SingleOrDefault(s => s.Name == name && s.Realms.Any(r => r.Name == realm));
        return Task.FromResult(scope);
    }

    public Task<List<Scope>> GetByNames(List<string> scopeNames, CancellationToken cancellationToken)
    {
        var result = _scopes
            .Where(s => scopeNames.Contains(s.Name))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<Scope>> GetByNames(string realm, List<string> scopeNames, CancellationToken cancellationToken)
    {
        var result = _scopes
            .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.Name == realm))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<Scope>> GetAllExposedScopes(string realm, CancellationToken cancellationToken)
    {
        var result = _scopes
            .Where(s => s.IsExposedInConfigurationEdp && s.Realms.Any(r => r.Name == realm))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<Scope>> GetAll(string realm, List<string> scopeNames, CancellationToken cancellationToken)
    {
        var result = _scopes
            .Where(s => scopeNames.Contains(s.Name) && s.Realms.Any(r => r.Name == realm))
            .ToList();
        return Task.FromResult(result);
    }

    public Task<List<Scope>> GetAllRealmScopes(string realm, CancellationToken cancellationToken)
    {
        var result = _scopes
            .Where(s => s.Component != null && s.Realms.Any(r => r.Name == realm))
            .ToList();
        return Task.FromResult(result);
    }

    public async Task<SearchResult<Scope>> Search(string realm, SearchScopeRequest request, CancellationToken cancellationToken)
    {
        var query = _scopes.AsQueryable()
            .Where(s => s.Realms.Any(r => r.Name == realm) && ((request.IsRole && s.Type == ScopeTypes.ROLE) || (!request.IsRole && (s.Type == ScopeTypes.IDENTITY || s.Type == ScopeTypes.APIRESOURCE))));
        if (!string.IsNullOrWhiteSpace(request.Filter))
            query = query.Where(request.Filter);
        if (!string.IsNullOrWhiteSpace(request.OrderBy))
            query = query.OrderBy(request.OrderBy);
        else
            query = query.OrderByDescending(s => s.UpdateDateTime);
        if (request.Protocols != null && request.Protocols.Any())
            query = query.Where(s => request.Protocols.Contains(s.Protocol));
        var nb = query.Count();
        var scopes = query.Skip(request.Skip.Value).Take(request.Take.Value).ToList();
        return await Task.FromResult(new SearchResult<Scope>
        {
            Count = nb,
            Content = scopes
        });
    }

    public void DeleteRange(IEnumerable<Scope> scopes)
    {
        foreach (var scope in scopes.ToList())
        {
            _scopes.Remove(scope);
        }
    }

    public void Add(Scope scope) => _scopes.Add(scope);

    public void Update(Scope scope) { }
}
