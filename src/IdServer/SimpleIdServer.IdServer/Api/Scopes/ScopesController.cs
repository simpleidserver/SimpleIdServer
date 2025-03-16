// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api.ApiResources;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.IntegrationEvents;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly IJwtBuilder _jwtBuilder;
    private readonly ILogger<ApiResourcesController> _logger;

    public ScopesController(
        IScopeRepository scopeRepository, 
        IRealmRepository realmRepository, 
        IApiResourceRepository apiResourceRepository, 
        IBusControl busControl, 
        ITokenRepository tokenRepository,
        IJwtBuilder jwtBuilder,
        ITransactionBuilder transactionBuilder,
        ILogger<ApiResourcesController> logger) : base(tokenRepository, jwtBuilder)
    {
        _scopeRepository = scopeRepository;
        _realmRepository = realmRepository;
        _apiResourceRepository = apiResourceRepository;
        _busControl = busControl;
        _jwtBuilder = jwtBuilder;
        _transactionBuilder = transactionBuilder;
        _logger = logger;
    }

    #region Querying

    [HttpPost]
    public async Task<IActionResult> Search([FromRoute] string prefix, [FromBody] SearchScopeRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
            var result = await _scopeRepository.Search(prefix, request, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
            var scope = await _scopeRepository.Get(prefix, id, cancellationToken);
            if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
            return new OkObjectResult(scope);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRealmScopes([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
            var scopes = await _scopeRepository.GetAllRealmScopes(prefix, cancellationToken);
            return new OkObjectResult(scopes);
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
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove scope"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    var scope = await _scopeRepository.Get(prefix, id, cancellationToken);
                    if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
                    _scopeRepository.DeleteRange(new List<Scope> { scope });
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Scope is removed");
                    return new NoContentResult();
                }
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromRoute] string prefix, [FromBody] Scope scope, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add scope"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    prefix = prefix ?? Constants.DefaultRealm;
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    var existingScope = await _scopeRepository.GetByName(prefix, scope.Name, cancellationToken);
                    if (existingScope != null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.ScopeAlreadyExists, scope.Name));
                    var realm = await _realmRepository.Get(prefix, cancellationToken);
                    scope.Id = Guid.NewGuid().ToString();
                    scope.Realms.Add(realm);
                    _scopeRepository.Add(scope);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Scope is added");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(scope),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }

    }

    [HttpPut]
    public async Task<IActionResult> Update([FromRoute] string prefix, string id, [FromBody] UpdateScopeRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Update scope"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    var scope = await _scopeRepository.Get(prefix, id, cancellationToken);
                    if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
                    scope.Description = request.Description;
                    scope.IsExposedInConfigurationEdp = request.IsExposedInConfigurationEdp;
                    scope.UpdateDateTime = DateTime.UtcNow;
                    _scopeRepository.Update(scope);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Scope is updated");
                    return new NoContentResult();
                }
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
    public async Task<IActionResult> AddClaimMapper([FromRoute] string prefix, string id, [FromBody] ScopeClaimMapper request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add claim mapping rule"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    var scope = await _scopeRepository.Get(prefix, id, cancellationToken);
                    if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
                    if (scope.ClaimMappers.Any(m => m.Name == request.Name))
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.ScopeClaimMapperNameMustBeUnique);
                    if (!string.IsNullOrWhiteSpace(request.TargetClaimPath) && scope.ClaimMappers.Any(m => m.TargetClaimPath == request.TargetClaimPath))
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.ScopeClaimMapperTokenClaimNameMustBeUnique);
                    if (!string.IsNullOrWhiteSpace(request.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == request.SAMLAttributeName))
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.ScopeClaimMapperSamlAttributeName);
                    if (IProvisioningMappingRule.IsUnique(request.MapperType) && scope.ClaimMappers.Any(r => r.MapperType == request.MapperType))
                        return BuildError(System.Net.HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.IdProvisioningTypeUnique);

                    request.Id = Guid.NewGuid().ToString();
                    scope.ClaimMappers.Add(request);
                    scope.UpdateDateTime = DateTime.UtcNow;
                    _scopeRepository.Update(scope);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Claim mapping rule is added");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(request),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveClaimMapper([FromRoute] string prefix, string id, string mapperId, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Remove scope claim mapper"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    var scope = await _scopeRepository.Get(prefix, id, cancellationToken);
                    if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
                    var scopeClaimMapper = scope.ClaimMappers.FirstOrDefault(m => m.Id == mapperId);
                    if (scopeClaimMapper == null)
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownScopeClaimMapper, mapperId));
                    scope.ClaimMappers.Remove(scopeClaimMapper);
                    _scopeRepository.Update(scope);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Claim mapper is removed");
                    return new NoContentResult();
                }
            }
            catch (OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                return BuildError(ex);
            }
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateClaimMapper([FromRoute] string prefix, string id, string mapperId, [FromBody] UpdateScopeClaimRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Add claim mapping rule"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    var scope = await _scopeRepository.Get(prefix, id, cancellationToken);
                    if (scope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
                    var scopeClaimMapper = scope.ClaimMappers.FirstOrDefault(m => m.Id == mapperId);
                    if (scopeClaimMapper == null)
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownScopeClaimMapper, mapperId));
                    if (!string.IsNullOrWhiteSpace(request.TargetClaimPath) && scope.ClaimMappers.Any(m => m.TargetClaimPath == request.TargetClaimPath && m.Id != mapperId))
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.ScopeClaimMapperTokenClaimNameMustBeUnique);
                    if (!string.IsNullOrWhiteSpace(request.SAMLAttributeName) && scope.ClaimMappers.Any(m => m.SAMLAttributeName == request.SAMLAttributeName && m.Id != mapperId))
                        throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.ScopeClaimMapperSamlAttributeName);
                    scopeClaimMapper.SourceUserAttribute = request.SourceUserAttribute;
                    scopeClaimMapper.SourceUserProperty = request.SourceUserProperty;
                    scopeClaimMapper.TargetClaimPath = request.TargetClaimPath;
                    scopeClaimMapper.SAMLAttributeName = request.SAMLAttributeName;
                    scopeClaimMapper.TokenClaimJsonType = request.TokenClaimJsonType;
                    scopeClaimMapper.IsMultiValued = request.IsMultiValued;
                    scopeClaimMapper.IncludeInAccessToken = request.IncludeInAccessToken;
                    scope.UpdateDateTime = DateTime.UtcNow;
                    _scopeRepository.Update(scope);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, "Claim mapping rule is added");
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(request),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
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
    public async Task<IActionResult> UpdateResources([FromRoute] string prefix, string id, [FromBody] UpdateScopeResourcesRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        using (var activity = Tracing.IdServerActivitySource.StartActivity($"Update API resources of the scope {id}"))
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    activity?.SetTag("realm", prefix);
                    activity?.SetTag("scope", id);
                    await CheckAccessToken(prefix, Config.DefaultScopes.Scopes.Name);
                    if (request == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, Global.InvalidIncomingRequest);
                    if (request.Resources == null) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, ScopeNames.Resources));
                    var existingScope = await _scopeRepository.Get(prefix, id, cancellationToken);
                    if (existingScope == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.NOT_FOUND, string.Format(Global.UnknownScope, id));
                    var existingApiResources = await _apiResourceRepository.GetByNames(prefix, request.Resources.ToList(), cancellationToken);
                    var unknownApiResources = request.Resources.Where(r => !existingApiResources.Any(er => er.Name == r));
                    if (unknownApiResources.Any()) throw new OAuthException(HttpStatusCode.BadRequest, ErrorCodes.INVALID_REQUEST, string.Format(Global.UnknownResource, string.Join(",", unknownApiResources)));
                    existingScope.ApiResources.Clear();
                    foreach (var apiResource in existingApiResources) existingScope.ApiResources.Add(apiResource);
                    _scopeRepository.Update(existingScope);
                    await transaction.Commit(cancellationToken);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"API resources have been updated for the scope {id}");
                    await _busControl.Publish(new UpdateScopeResourcesSuccessEvent
                    {
                        Realm = prefix,
                        Name = existingScope.Name,
                        Resources = request.Resources.ToList()
                    });
                    return new NoContentResult();
                }
            }
            catch (OAuthException ex)
            {
                _logger.LogError(ex.ToString());
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new UpdateScopeResourcesFailureEvent
                {
                    Realm = prefix,
                    Id = id,
                    Resources = request.Resources?.ToList()
                });
                return BuildError(ex);
            }
        }
    }

    #endregion
}