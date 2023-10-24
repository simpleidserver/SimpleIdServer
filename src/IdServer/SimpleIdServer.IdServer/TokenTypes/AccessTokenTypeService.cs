// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Jwt;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.TokenTypes;

public class AccessTokenTypeService : ITokenTypeService
{
    private readonly IJwtBuilder _jwtBuilder;
    private readonly IdServerHostOptions _options;

    public AccessTokenTypeService(IJwtBuilder jwtBuilder, IOptions<IdServerHostOptions> options)
    {
        _jwtBuilder = jwtBuilder;
        _options = options.Value;
    }

    public const string NAME = "urn:ietf:params:oauth:token-type:access_token";
    public string Name => NAME;
    public string TokenType => TokenResponseParameters.AccessToken;

    public TokenResult Parse(string realm, string token)
    {
        var extractionResult = _jwtBuilder.ReadSelfIssuedJsonWebToken(realm, token);
        if (extractionResult.Error != null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, extractionResult.Error);
        var claims = extractionResult.Jwt.GetClaimsDic();
        var subject = string.Empty;
        if (claims.ContainsKey(OpenIdConnectParameterNames.ClientId)) subject = claims[OpenIdConnectParameterNames.ClientId].ToString();
        return new TokenResult
        {
            Claims = extractionResult.Jwt.GetClaimsDic(),
            Subject = subject
        };
    }

    public async Task<string> Build(string realm, string issuer, Client client, Dictionary<string, object> claims, CancellationToken cancellationToken)
    {
        var currentDateTime = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            IssuedAt = currentDateTime,
            Expires = currentDateTime.AddSeconds(client.TokenExpirationTimeInSeconds ?? _options.DefaultTokenExpirationTimeInSeconds),
            Claims = claims
        };
        var accessToken = await _jwtBuilder.BuildAccessToken(realm, client, tokenDescriptor, cancellationToken);
        return accessToken;
    }
}
