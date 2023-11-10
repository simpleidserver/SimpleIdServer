// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.ApiResources;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Scopes;

public class ScopesController : BaseController
{
    private readonly IScopeRepository _scopeRepository;
    private readonly IRealmRepository _realmRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IBusControl _busControl;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly ILogger<ApiResourcesController> _logger;

    public ScopesController(IScopeRepository scopeRepository, IRealmRepository realmRepository, IApiResourceRepository apiResourceRepository, IBusControl busControl, IJwtBuilder jwtBuilder, ILogger<ApiResourcesController> logger)
    {
        _scopeRepository = scopeRepository;
        _realmRepository = realmRepository;
        _apiResourceRepository = apiResourceRepository;
        _busControl = busControl;
        _jwtBuilder = jwtBuilder;
        _logger = logger;
    }

    #region Querying

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchScopeRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
            IQueryable<Scope> query = _scopeRepository.Query()
                .Include(p => p.Realms)
                .Where(p => p.Realms.Any(r => r.Name == prefix))
                .AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.Filter))
                query = query.Where(request.Filter);

            if (!string.IsNullOrWhiteSpace(request.OrderBy))
                query = query.OrderBy(request.OrderBy);

            if (request.Protocols != null && request.Protocols.Any())
                query = query.Where(q => request.Protocols.Contains(q.Protocol));

            var nb = query.Count();
            var clients = await query.Skip(request.Skip.Value).Take(request.Take.Value).ToListAsync();
            return new OkObjectResult(new SearchResult<Scope>
            {
                Count = nb,
                Content = clients
            });
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromRoute] string prefix, [FromBody] string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
            var scope = await _scopeRepository.Query()
                .Include(p => p.Realms)
                .Include(p => p.ClaimMappers)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Realms.Any(r => r.Name == prefix) && p.Id == id);
            if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, id));
            return new OkObjectResult(scope);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    #endregion

    #region CRUD

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove scope"))
        {
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                var scope = await _scopeRepository.Query()
                    .Include(u => u.Realms)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.Name == prefix));
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, id));
                _scopeRepository.DeleteRange(new List<Scope> { scope });
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, "Scope is removed");
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromRoute] Scope scope)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add scope"))
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                var existingScope = await _scopeRepository.Query()
                    .Include(s => s.Realms)
                    .AsNoTracking()
                    .AnyAsync(s => s.Name == scope.Name && s.Realms.Any(r => r.Name == prefix), CancellationToken.None);
                if (existingScope) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.SCOPE_ALREADY_EXISTS, scope.Name));
                var realm = await _realmRepository.Query().SingleAsync(r => r.Name == prefix);
                scope.Id = Guid.NewGuid().ToString();
                scope.Realms.Add(realm);
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, "Scope is added");
                return new ContentResult
                {
                    Content = JsonSerializer.Serialize(scope),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }

    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UpdateScopeRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Update scope"))
        {
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                var scope = await _scopeRepository.Query()
                    .Include(u => u.Realms)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.Name == prefix));
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, id));
                scope.Description = request.Description;
                scope.IsExposedInConfigurationEdp = request.IsExposedInConfigurationEdp;
                scope.UpdateDateTime = DateTime.UtcNow;
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, "Scope is updated");
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    #endregion

    #region Mappers

    [HttpPost]
    public async Task<IActionResult> AddClaimMapper([FromRoute] string prefix, string id, [FromBody] ScopeClaimMapper request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add claim mapping rule"))
        {
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                var scope = await _scopeRepository.Query()
                    .Include(u => u.ClaimMappers)
                    .Include(u => u.Realms)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.Name == prefix));
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, id));
                if(scope.ClaimMappers.Any(m => m.Name == request.Name)) 
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SCOPE_CLAIM_MAPPER_NAME_MUSTBEUNIQUE);
                if (!string.IsNullOrWhiteSpace(request.TargetClaimPath) && scope.ClaimMappers.Any(m => m.TargetClaimPath == request.TargetClaimPath))
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SCOPE_CLAIM_MAPPER_TOKENCLAIMNAME_MUSTBEUNIQUE);
                if (!string.IsNullOrWhiteSpace(request.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == request.SAMLAttributeName))
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SCOPE_CLAIM_MAPPER_SAML_ATTRIBUTE_NAME);

                request.Id = Guid.NewGuid().ToString();
                scope.ClaimMappers.Add(request);
                scope.UpdateDateTime = DateTime.UtcNow;
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, "Claim mapping rule is added");
                return new ContentResult
                {
                    Content = JsonSerializer.Serialize(request),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveClaimMapper([FromRoute] string prefix, string id, string mapperId)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove scope claim mapper"))
        {
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                var scope = await _scopeRepository.Query()
                    .Include(u => u.Realms)
                    .Include(u => u.ClaimMappers)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.Name == prefix));
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, id));
                var scopeClaimMapper = scope.ClaimMappers.FirstOrDefault(m => m.Id == mapperId);
                if(scopeClaimMapper == null)
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_SCOPE_CLAIM_MAPPER, mapperId));
                scope.ClaimMappers.Remove(scopeClaimMapper);
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, "Claim mapper is removed");
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateClaimMapper([FromRoute] string prefix, string id, string mapperId, [FromBody] UpdateScopeClaimRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add claim mapping rule"))
        {
            try
            {
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                var scope = await _scopeRepository.Query()
                    .Include(u => u.ClaimMappers)
                    .Include(u => u.Realms)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Realms.Any(r => r.Name == prefix));
                if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, id));
                var scopeClaimMapper = scope.ClaimMappers.FirstOrDefault(m => m.Id == mapperId);
                if (scopeClaimMapper == null)
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_SCOPE_CLAIM_MAPPER, mapperId));
                if (!string.IsNullOrWhiteSpace(request.TargetClaimPath) && scope.ClaimMappers.Any(m => m.TargetClaimPath == request.TargetClaimPath))
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SCOPE_CLAIM_MAPPER_TOKENCLAIMNAME_MUSTBEUNIQUE);
                if (!string.IsNullOrWhiteSpace(request.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == request.SAMLAttributeName))
                    throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.SCOPE_CLAIM_MAPPER_SAML_ATTRIBUTE_NAME);
                scopeClaimMapper.SourceUserAttribute = request.SourceUserAttribute;
                scopeClaimMapper.SourceUserProperty = request.SourceUserProperty;
                scopeClaimMapper.TargetClaimPath = request.TargetClaimPath;
                scopeClaimMapper.SAMLAttributeName = request.SAMLAttributeName;
                scopeClaimMapper.TokenClaimJsonType = request.TokenClaimJsonType;
                scopeClaimMapper.IsMultiValued = request.IsMultiValued;
                scope.UpdateDateTime = DateTime.UtcNow;
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, "Claim mapping rule is added");
                return new ContentResult
                {
                    Content = JsonSerializer.Serialize(request),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    #endregion

    #region Resources

    [HttpPut]
    public async Task<IActionResult> UpdateResources([FromRoute] string prefix, string name, [FromBody] UpdateScopeResourcesRequest request)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity($"Update API resources of the scope {name}"))
        {
            try
            {
                activity?.SetTag("realm", prefix);
                activity?.SetTag("scope", name);
                CheckAccessToken(prefix, Constants.StandardScopes.Scopes.Name, _jwtBuilder);
                if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, ErrorMessages.INVALID_INCOMING_REQUEST);
                if (request.Resources == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, ScopeNames.Resources));
                var existingScope = await _scopeRepository.Query().Include(s => s.Realms).Include(s => s.ApiResources).FirstOrDefaultAsync(s => s.Name == name && s.Realms.Any(r => r.Name == prefix));
                if (existingScope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(ErrorMessages.UNKNOWN_SCOPE, name));
                var existingApiResources = await _apiResourceRepository.Query().Include(r => r.Realms).Where(r => request.Resources.Contains(r.Name) && r.Realms.Any(re => re.Name == prefix)).ToListAsync();
                var unknownApiResources = request.Resources.Where(r => !existingApiResources.Any(er => er.Name == r));
                if (unknownApiResources.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.UNKNOWN_RESOURCE, string.Join(",", unknownApiResources)));
                existingScope.ApiResources.Clear();
                foreach (var apiResource in existingApiResources) existingScope.ApiResources.Add(apiResource);
                await _scopeRepository.SaveChanges(CancellationToken.None);
                activity?.SetStatus(ActivityStatusCode.Ok, $"API resources have been updated for the scope {name}");
                await _busControl.Publish(new UpdateScopeResourcesSuccessEvent
                {
                    Realm = prefix,
                    Name = name,
                    Resources = request.Resources.ToList()
                });
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new UpdateScopeResourcesFailureEvent
                {
                    Realm = prefix,
                    Name = name,
                    Resources = request.Resources?.ToList()
                });
                return BuildError(ex);
            }
        }
    }

    #endregion
}