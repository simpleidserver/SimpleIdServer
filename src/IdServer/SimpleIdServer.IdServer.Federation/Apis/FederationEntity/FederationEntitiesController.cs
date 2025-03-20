// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Federation.Migrations;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation.Clients;
using SimpleIdServer.OpenidFederation.Stores;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Federation.Apis.FederationEntity;

public class FederationEntitiesController : BaseController
{
    private readonly IFederationEntityStore _federationEntityStore;
    private readonly ITransactionBuilder _transactionBuilder;
    private readonly ILogger<FederationEntitiesController> _logger;
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;

    public FederationEntitiesController(
        IFederationEntityStore federationEntityStore,
        ITransactionBuilder transactionBuilder,
        ILogger<FederationEntitiesController> logger,
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        ITokenRepository tokenRepository, 
        IJwtBuilder jwtBuilder) : base(tokenRepository, jwtBuilder)
    {
        _federationEntityStore = federationEntityStore;
        _transactionBuilder = transactionBuilder;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> AddTrustAnchor([FromRoute] string prefix, [FromBody] AddTrustedAnchorRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, ConfigureIdServerFederationDataseeder.FederationEntitiesScope.Name);
            if (request == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, IdServer.Resources.Global.InvalidIncomingRequest);
            if (string.IsNullOrWhiteSpace(request.Url)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(IdServer.Resources.Global.MissingParameter, "url"));
            var federationEntity = await _federationEntityStore.GetByUrl(prefix, request.Url, cancellationToken);
            if (federationEntity != null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Resources.Global.FederationEntityAlreadyExists);
            using(var resolver = TrustChainResolver.New(_httpClientFactory.GetHttpClient()))
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    var openidFederation = await resolver.ResolveOpenidFederation(request.Url, cancellationToken);
                    if (openidFederation == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Resources.Global.OpenidFederationCannotBeResolved);
                    federationEntity = new SimpleIdServer.OpenidFederation.Domains.FederationEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        IsSubordinate = false,
                        Realm = prefix,
                        Sub = request.Url,
                        CreateDateTime = DateTime.UtcNow
                    };
                    _federationEntityStore.Add(federationEntity);
                    await transaction.Commit(cancellationToken);
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(federationEntity),
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.Created
                    };
                }
            }
        }
        catch(InvalidOperationException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromRoute] string prefix, string id, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            using (var transaction = _transactionBuilder.Build())
            {
                await CheckAccessToken(prefix, ConfigureIdServerFederationDataseeder.FederationEntitiesScope.Name);
                var federationEntity = await _federationEntityStore.Get(id, cancellationToken);
                if (federationEntity == null) throw new OAuthException(HttpStatusCode.NotFound, ErrorCodes.INVALID_REQUEST, Resources.Global.FederationEntityUnknown);
                _federationEntityStore.Remove(federationEntity);
                await transaction.Commit(cancellationToken);
                return NoContent();
            }
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SearchFederationEntities([FromRoute] string prefix, [FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        try
        {
            await CheckAccessToken(prefix, ConfigureIdServerFederationDataseeder.FederationEntitiesScope.Name);
            var result = await _federationEntityStore.Search(prefix, request, cancellationToken);
            return new OkObjectResult(result);
        }
        catch (OAuthException ex)
        {
            _logger.LogError(ex.ToString());
            return BuildError(ex);
        }
    }
}