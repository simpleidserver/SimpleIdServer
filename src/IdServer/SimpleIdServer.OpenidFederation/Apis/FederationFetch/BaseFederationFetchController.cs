// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Clients;
using SimpleIdServer.OpenidFederation.Resources;
using SimpleIdServer.OpenidFederation.Stores;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.OpenidFederation.Apis.FederationFetch;

public abstract class BaseFederationFetchController : BaseOpenidFederationController
{
    private readonly IFederationEntityStore _federationEntityStore;
    private readonly IDistributedCache _distributedCache;
    private readonly IdServer.Helpers.IHttpClientFactory _httpClientFactory;

    public BaseFederationFetchController(
        IFederationEntityStore federationEntityStore,
        IDistributedCache distributedCache,
        IdServer.Helpers.IHttpClientFactory httpClientFactory)
    {
        _federationEntityStore = federationEntityStore;
        _distributedCache = distributedCache;
        _httpClientFactory = httpClientFactory;
    }

    protected async Task<IActionResult> Get(FederationFetchRequest request, string realm, string issuer, CancellationToken cancellationToken)
    {
        var signingCredential = GetSigningCredential(realm);
        if (signingCredential == null) return Error(HttpStatusCode.InternalServerError, ErrorCodes.INVALID_ISSUER, Global.CannotExtractSignatureKey);
        var validationResult = await Validate(
            request, 
            signingCredential,
            realm,
            issuer,
            cancellationToken);
        if (validationResult.ErrorCode != null) return Error(validationResult.HttpStatusCode, validationResult.ErrorCode, validationResult.ErrorMessage);
        var handler = new JsonWebTokenHandler();
        var jws = handler.CreateToken(JsonSerializer.Serialize(validationResult.OpenidFederationResult), new SigningCredentials(signingCredential.Key, signingCredential.Algorithm));
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            Content = jws,
            ContentType = OpenidFederationConstants.EntityStatementContentType
        };
    }

    protected abstract Task<OpenidFederationResult> BuildSelfIssuedFederationEntity(BuildFederationEntityRequest request, CancellationToken cancellationToken);

    protected abstract SigningCredentials GetSigningCredential(string realm);

    private async Task<FederationFetchValidationResult> Validate(
        FederationFetchRequest request, 
        SigningCredentials signingCredentials, 
        string realm,
        string issuer, 
        CancellationToken cancellationToken)
    {
        if (request == null) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_REQUEST, Resources.Global.InvalidIncomingRequest);
        if (string.IsNullOrWhiteSpace(request.Iss)) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_REQUEST, string.Format(Resources.Global.MissingParameter, "iss"));
        if (request.Iss != issuer) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_ISSUER, Resources.Global.InvalidIssuer);
        if(!string.IsNullOrWhiteSpace(request.Sub))
        {
            var cacheKey = GetCacheKey(request.Sub);
            var entityStatement = await _federationEntityStore.GetSubordinate(request.Sub, realm, cancellationToken);
            if (entityStatement == null) return FederationFetchValidationResult.Error(ErrorCodes.NOT_FOUND, string.Format(Resources.Global.UnknownEntityStatement, request.Sub));
            var cacheOpenidFederation = await _distributedCache.GetAsync(cacheKey, cancellationToken);
            if (cacheOpenidFederation == null)
            {
                using (var resolver = TrustChainResolver.New(_httpClientFactory.GetHttpClient()))
                {
                    var openidFederation = await resolver.ResolveOpenidFederation(request.Sub, cancellationToken);
                    if(openidFederation == null) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_REQUEST, Resources.Global.ImpossibleToExtractOpenidFederation);
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(openidFederation), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = openidFederation.FederationResult.ValidTo
                    }, cancellationToken);
                    openidFederation.FederationResult.Iss = issuer;
                    return FederationFetchValidationResult.Ok(openidFederation.FederationResult);
                }
            }

            return FederationFetchValidationResult.Ok(JsonSerializer.Deserialize<OpenidFederationResult>(cacheOpenidFederation));
        }

        var selfIssuedEntityStatement = await BuildSelfIssuedFederationEntity(new BuildFederationEntityRequest
        {
            Credential = signingCredentials,
            Issuer = GetAbsoluteUriWithVirtualPath(Request),
            Realm = realm
        }, cancellationToken);
        return FederationFetchValidationResult.Ok(selfIssuedEntityStatement);
    }

    private static string GetCacheKey(string sub)
        => $"OpenidFederation_{sub}";

    private class FederationFetchValidationResult
    {
        private FederationFetchValidationResult()
        {
            
        }

        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public OpenidFederationResult OpenidFederationResult { get; private set; }

        public static FederationFetchValidationResult Error(string code, string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
            => new FederationFetchValidationResult { ErrorCode = code, ErrorMessage = message, HttpStatusCode = httpStatusCode };

        public static FederationFetchValidationResult Ok(OpenidFederationResult openidFederationResult)
            => new FederationFetchValidationResult { OpenidFederationResult = openidFederationResult };
    }
}