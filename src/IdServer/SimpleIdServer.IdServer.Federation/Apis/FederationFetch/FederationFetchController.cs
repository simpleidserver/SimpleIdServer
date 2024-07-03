// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Federation.Builders;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation.Apis.FederationFetch;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.IdServer.Federation.Apis.FederationFetch;

public class FederationFetchController : BaseFederationFetchController
{
    private readonly IFederationEntityBuilder _federationEntityBuilder;
    private readonly IKeyStore _keyStore;
    private readonly OpenidFederationOptions _options;

    public FederationFetchController(
        IFederationEntityBuilder federationEntityBuilder,
        IKeyStore keyStore,
        IFederationEntityStore federationEntityStore,
        IDistributedCache distributedCache,
        IHttpClientFactory httpClientFactory,
        IOptions<OpenidFederationOptions> options) : base(federationEntityStore, distributedCache, httpClientFactory)
    {
        _federationEntityBuilder = federationEntityBuilder;
        _keyStore = keyStore;
        _options = options.Value;
    }

    [HttpGet]
    public Task<IActionResult> Get(
        [FromRoute] string prefix, 
        [FromQuery] FederationFetchRequest request, 
        CancellationToken cancellationToken)
    {
        prefix = prefix ?? Constants.DefaultRealm;
        var issuer = Request.GetAbsoluteUriWithVirtualPath();
        return Get(request, prefix, issuer, cancellationToken);
    }

    protected override Task<OpenidFederationResult> BuildSelfIssuedFederationEntity(BuildFederationEntityRequest request, CancellationToken cancellationToken)
        => _federationEntityBuilder.BuildSelfIssued(request, cancellationToken);

    protected override SigningCredentials GetSigningCredential(string realm)
    {
        var signingKeys = _keyStore.GetAllSigningKeys(realm);
        var signingKey = signingKeys.FirstOrDefault(k => k.Kid == _options.TokenSignedKid);
        return signingKey;
    }
}