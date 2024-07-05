// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Federation.Builders;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Federation.Apis.OpenidFederation;

public class OpenidFederationController : BaseOpenidFederationController
{
    private readonly IKeyStore _keyStore;
    private readonly IOpenidProviderFederationEntityBuilder _federationEntityBuilder;
    private readonly OpenidFederationOptions _options;

    public OpenidFederationController(
        IKeyStore keyStore,
        IOpenidProviderFederationEntityBuilder federationEntityBuilder,
        IOptions<OpenidFederationOptions> options)
    {
        _keyStore = keyStore;
        _federationEntityBuilder = federationEntityBuilder;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
    {
        var realm = prefix ?? Constants.DefaultRealm;
        var issuer = GetAbsoluteUriWithVirtualPath(Request);
        var signingKeys = _keyStore.GetAllSigningKeys(realm);
        var signingKey = signingKeys.FirstOrDefault(k => k.Kid == _options.TokenSignedKid);
        if (signingKey == null) return Error(System.Net.HttpStatusCode.InternalServerError, SimpleIdServer.OpenidFederation.ErrorCodes.INTERNAL_SERVER_ERROR, SimpleIdServer.OpenidFederation.Resources.Global.CannotExtractSignatureKey);
        var selfIssuedFederationEntity = await _federationEntityBuilder.BuildSelfIssued(new SimpleIdServer.OpenidFederation.Builders.BuildFederationEntityRequest
        {
            Credential = signingKey,
            Issuer =  issuer,
            Realm = realm
        }, cancellationToken);
        var handler = new JsonWebTokenHandler();
        var jws = handler.CreateToken(JsonSerializer.Serialize(selfIssuedFederationEntity), new SigningCredentials(signingKey.Key, signingKey.Algorithm));
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            Content = jws,
            ContentType = OpenidFederationConstants.EntityStatementContentType
        };
    }
}