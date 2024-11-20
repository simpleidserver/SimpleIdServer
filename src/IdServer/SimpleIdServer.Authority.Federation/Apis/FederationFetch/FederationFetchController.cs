// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Authority.Federation.Builders;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis.FederationFetch;
using SimpleIdServer.OpenidFederation.Apis.OpenidFederation;
using SimpleIdServer.OpenidFederation.Builders;
using SimpleIdServer.OpenidFederation.Stores;

namespace SimpleIdServer.Authority.Federation.Apis.FederationFetch;

[Route(OpenidFederationConstants.EndPoints.FederationFetch)]
public class FederationFetchController : BaseFederationFetchController
{
    private readonly IAuthorityFederationEntityBuilder _federationEntityBuilder;
    private readonly AuthorityFederationOptions _options;

    public FederationFetchController(
        IAuthorityFederationEntityBuilder federationEntityBuilder,
        IFederationEntityStore federationEntityStore,
        IDistributedCache distributedCache,
        IdServer.Helpers.IHttpClientFactory httpClientFactory,
        IOptions<AuthorityFederationOptions> options) : base(federationEntityStore, distributedCache, httpClientFactory)
    {
        _federationEntityBuilder = federationEntityBuilder;
        _options = options.Value;
    }

    [HttpGet]
    public Task<IActionResult> Get(
        [FromQuery] FederationFetchRequest request,
        CancellationToken cancellationToken)
    {
        var issuer = this.GetAbsoluteUriWithVirtualPath();
        return Get(request, string.Empty, issuer, cancellationToken);
    }

    protected override Task<OpenidFederationResult> BuildSelfIssuedFederationEntity(BuildFederationEntityRequest request, CancellationToken cancellationToken)
        => _federationEntityBuilder.BuildSelfIssued(request, cancellationToken);

    protected override SigningCredentials GetSigningCredential(string realm)
    {
        return _options.SigningCredentials;
    }
}