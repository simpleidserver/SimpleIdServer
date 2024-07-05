// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis;
using SimpleIdServer.Rp.Federation.Builders;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.Rp.Federation.Apis.OpenidFederation;

[Route(OpenidFederationConstants.EndPoints.OpenidFederation)]
public class OpenidFederationController : BaseOpenidFederationController
{
    private readonly RpFederationOptions _options;
    private readonly IRpFederationEntityBuilder _federationEntityBuilder;

    public OpenidFederationController(
        IOptions<RpFederationOptions> options,
        IRpFederationEntityBuilder federationEntityBuilder)
    {
        _options = options.Value;
        _federationEntityBuilder = federationEntityBuilder;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var issuer = GetAbsoluteUriWithVirtualPath(Request);
        if (_options.SigningCredentials == null) return Error(System.Net.HttpStatusCode.InternalServerError, SimpleIdServer.OpenidFederation.ErrorCodes.INTERNAL_SERVER_ERROR, SimpleIdServer.OpenidFederation.Resources.Global.CannotExtractSignatureKey);
        var selfIssuedFederationEntity = await _federationEntityBuilder.BuildSelfIssued(new SimpleIdServer.OpenidFederation.Builders.BuildFederationEntityRequest
        {
            Credential = _options.SigningCredentials,
            Issuer = issuer
        }, cancellationToken);
        var handler = new JsonWebTokenHandler();
        var jws = handler.CreateToken(JsonSerializer.Serialize(selfIssuedFederationEntity), new SigningCredentials(_options.SigningCredentials.Key, _options.SigningCredentials.Algorithm));
        return new ContentResult
        {
            StatusCode = (int)HttpStatusCode.OK,
            Content = jws,
            ContentType = OpenidFederationConstants.EntityStatementContentType
        };
    }
}