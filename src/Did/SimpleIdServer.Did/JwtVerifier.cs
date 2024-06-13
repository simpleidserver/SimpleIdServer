// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did;

public interface IJwtVerifier
{
    Task<TokenValidationResult> VerifyWithJsonWebKey(string jwtToken, CancellationToken cancellationToken);
}

public class JwtVerifier : IJwtVerifier
{
    private readonly IDidFactoryResolver _resolver;

    public JwtVerifier(IDidFactoryResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<TokenValidationResult> VerifyWithJsonWebKey(string jwtToken, CancellationToken cancellationToken)
    {
        var handler = new JsonWebTokenHandler();
        if (!handler.CanReadToken(jwtToken)) throw new InvalidOperationException("cannot read the Json Web Token");
        var jwt = handler.ReadJsonWebToken(jwtToken);
        var kid = jwt.Kid;
        var did = await _resolver.Resolve(kid, cancellationToken);
        var firstVerificationMethod = did.VerificationMethod.First();
        var publicKeyJwk = firstVerificationMethod.PublicKeyJwk;
        var result = await handler.ValidateTokenAsync(jwtToken, new TokenValidationParameters
        {
            IssuerSigningKey = publicKeyJwk,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        });
        return result;
    }
}
