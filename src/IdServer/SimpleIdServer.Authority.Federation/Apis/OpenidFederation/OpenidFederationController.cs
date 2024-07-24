// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Authority.Federation.Builders;
using SimpleIdServer.OpenidFederation;
using SimpleIdServer.OpenidFederation.Apis;
using System.Net;
using System.Text.Json;

namespace SimpleIdServer.Authority.Federation.Apis.OpenidFederation;

[Route(OpenidFederationConstants.EndPoints.OpenidFederation)]
public class OpenidFederationController : BaseOpenidFederationController
{
    private readonly AuthorityFederationOptions _options;
    private readonly IAuthorityFederationEntityBuilder _authorityFederationEntityBuilder;

    public OpenidFederationController(
        IOptions<AuthorityFederationOptions> options,
        IAuthorityFederationEntityBuilder authorityFederationEntityBuilder)
    {
        _options = options.Value;
        _authorityFederationEntityBuilder = authorityFederationEntityBuilder;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var issuer = this.GetAbsoluteUriWithVirtualPath();
        if (_options.SigningCredentials == null) return Error(System.Net.HttpStatusCode.InternalServerError, SimpleIdServer.OpenidFederation.ErrorCodes.INTERNAL_SERVER_ERROR, SimpleIdServer.OpenidFederation.Resources.Global.CannotExtractSignatureKey);
        var selfIssuedFederationEntity = await _authorityFederationEntityBuilder.BuildSelfIssued(new SimpleIdServer.OpenidFederation.Builders.BuildFederationEntityRequest
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