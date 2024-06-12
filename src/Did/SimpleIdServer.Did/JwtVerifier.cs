// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto.SecurityKeys;
using SimpleIdServer.Did.Encoders;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did;

public interface IJwtVerifier
{

}

public class JwtVerifier : IJwtVerifier
{
    private readonly IDidFactoryResolver _resolver;
    private readonly IVerificationMethodEncoding _verificationMethodEncoding;

    public JwtVerifier(IDidFactoryResolver resolver, IVerificationMethodEncoding verificationMethodEncoding)
    {
        _resolver = resolver;
        _verificationMethodEncoding = verificationMethodEncoding;
    }

    public async Task Verify(string jwtToken, CancellationToken cancellationToken)
    {
        // https://www.npmjs.com/package/@cef-ebsi/key-did-resolver
        var handler = new JsonWebTokenHandler();
        if (!handler.CanReadToken(jwtToken)) throw new InvalidOperationException("cannot read the Json Web Token");
        var jwt = handler.ReadJsonWebToken(jwtToken);
        var kid = jwt.Kid;
        var did = await _resolver.Resolve(kid, cancellationToken);
        var firstVerificationMethod = did.VerificationMethod.First();
        var asymKey = _verificationMethodEncoding.Decode(firstVerificationMethod);
        var result = handler.ValidateToken(jwtToken, new TokenValidationParameters
        {
            IssuerSigningKey = asymKey.BuildSigningCredentials().Key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        });
        string ss = "";
    }
}
