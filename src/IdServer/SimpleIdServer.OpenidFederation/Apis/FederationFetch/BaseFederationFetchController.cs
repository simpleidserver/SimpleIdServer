// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Stores;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.OpenidFederation.Apis.FederationFetch;

public class BaseFederationFetchController : Controller
{
    private readonly IFederationEntityStore _federationEntityStore;
    private readonly IDistributedCache _distributedCache;
    private readonly IHttpClientFactory _httpClientFactory;

    public BaseFederationFetchController(
        IFederationEntityStore federationEntityStore,
        IDistributedCache distributedCache,
        IHttpClientFactory httpClientFactory)
    {
        _federationEntityStore = federationEntityStore;
        _distributedCache = distributedCache;
        _httpClientFactory = httpClientFactory;
    }

    protected async Task Get(FederationFetchRequest request, string issuer, CancellationToken cancellationToken)
    {
        var validationResult = await Validate(request, issuer, cancellationToken);
        await _distributedCache.GetAsync($"OpenidFederation_{request.Sub}", cancellationToken);

    }

    private async Task<FederationFetchValidationResult> Validate(FederationFetchRequest request, string issuer, CancellationToken cancellationToken)
    {
        if (request == null) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_REQUEST, Resources.Global.InvalidIncomingRequest);
        if (string.IsNullOrWhiteSpace(request.Iss)) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_REQUEST, string.Format(Resources.Global.MissingParameter, "iss"));
        if (request.Iss != issuer) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_ISSUER, Resources.Global.InvalidIssuer);
        if(!string.IsNullOrWhiteSpace(request.Sub))
        {
            var cacheKey = $"OpenidFederation_{request.Sub}";
            var entityStatement = await _federationEntityStore.Get(request.Sub, cancellationToken);
            if (entityStatement == null) return FederationFetchValidationResult.Error(ErrorCodes.NOT_FOUND, Resources.Global.UnknownEntityStatement);
            var cacheOpenidFederation = await _distributedCache.GetAsync(cacheKey, cancellationToken);
            if (cacheOpenidFederation == null)
            {
                using (var resolver = TrustChainResolver.New(_httpClientFactory.CreateClient()))
                {
                    var openidFederation = await resolver.ResolveOpenidFederation(request.Sub, cancellationToken);
                    if(openidFederation == null) return FederationFetchValidationResult.Error(ErrorCodes.INVALID_REQUEST, Resources.Global.ImpossibleToExtractOpenidFederation);
                    await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(openidFederation), new DistributedCacheEntryOptions
                    {
                        // AbsoluteExpiration = openidFederation.Exp, // TODO
                    }, cancellationToken);
                    return FederationFetchValidationResult.Ok(openidFederation);
                }
            }

            return FederationFetchValidationResult.Ok(JsonSerializer.Deserialize<OpenidFederationResult>(cacheOpenidFederation));
        }

        // SELF !!
        return FederationFetchValidationResult.Ok(null);
    }

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