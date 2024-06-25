// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Jwt;

public class DidJsonWebTokenHandler
{
    private readonly IVerificationMethodEncoding _verificationMethodEncoding;

    public DidJsonWebTokenHandler(
        IVerificationMethodEncoding verificationMethodEncoding)
    {
        _verificationMethodEncoding = verificationMethodEncoding;
    }

    public static DidJsonWebTokenHandler New()
    {
        return new DidJsonWebTokenHandler(
            new VerificationMethodEncoding(VerificationMethodStandardFactory.GetAll(), MulticodecSerializerFactory.Build(), MulticodecSerializerFactory.AllVerificationMethods)
        );
    }

    public string Secure(
        Dictionary<string, object> claims,
        DidDocument didDocument,
        string verificationMethodId)
    {
        if (claims == null) throw new ArgumentNullException(nameof(claims));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (string.IsNullOrWhiteSpace(verificationMethodId)) throw new ArgumentNullException(nameof(verificationMethodId));
        var verificationMethod = didDocument.VerificationMethod.SingleOrDefault(m => m.Id == verificationMethodId);
        if (verificationMethod == null) throw new ArgumentException($"The verification method {verificationMethodId} doesn't exist");
        var asymKey = _verificationMethodEncoding.Decode(verificationMethod);
        return Secure(claims, asymKey);
    }

    public string Secure(
        Dictionary<string, object> claims,
        IAsymmetricKey asymmKey,
        string tokenType = null,
        string kid = null)
    {
        if (claims == null) throw new ArgumentNullException(nameof(claims));
        if (asymmKey == null) throw new ArgumentNullException(nameof(asymmKey));
        var signingCredentials = asymmKey.BuildSigningCredentials(kid);
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            TokenType = tokenType,
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = signingCredentials
        };
        securityTokenDescriptor.Claims = claims;
        var handler = new JsonWebTokenHandler();
        var result = handler.CreateToken(securityTokenDescriptor);
        return result;
    }

    public async Task<bool> CheckJwt(
        string jwtToken,
        IDidFactoryResolver didFactoryResolver,
        CancellationToken cancellationToken)
    {
        var handler = new JsonWebTokenHandler();
        if (!handler.CanReadToken(jwtToken)) throw new InvalidOperationException("cannot read the Json Web Token");
        var jwt = handler.ReadJsonWebToken(jwtToken);
        var kid = jwt.Kid;
        var did = await didFactoryResolver.Resolve(kid, cancellationToken);
        return CheckJwt(jwtToken, did);
    }

    public bool CheckJwt(
        string jwt,
        DidDocument didDocument)
    {
        if (string.IsNullOrWhiteSpace(jwt)) throw new ArgumentNullException(nameof(jwt));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (didDocument.AssertionMethod == null) throw new InvalidOperationException("There is no assertion method");
        var assertionIds = didDocument.AssertionMethod.Select(m => m.ToString());
        if (!assertionIds.Any()) throw new InvalidOperationException("There is no assertion method");
        var assertionMethods = didDocument.VerificationMethod.Where(m => assertionIds.Contains(m.Id));
        var handler = new JsonWebTokenHandler();
        var jsonWebToken = handler.ReadJsonWebToken(jwt);
        var content = System.Text.Encoding.UTF8.GetBytes($"{jsonWebToken.EncodedHeader}.{jsonWebToken.EncodedPayload}");
        var signature = Base64UrlEncoder.DecodeBytes(jsonWebToken.EncodedSignature);
        foreach (var assertionMethod in assertionMethods)
        {
            var asymKey = _verificationMethodEncoding.Decode(assertionMethod);
            if (asymKey.CheckHash(content, signature, HashAlgorithmName.SHA256)) return true;
        }

        return false;
    }

    public bool CheckJwt(
        string jwt,
        DidDocument didDocument,
        string verificationMethodId)
    {
        if (string.IsNullOrWhiteSpace(jwt)) throw new ArgumentNullException(nameof(jwt));
        if (didDocument == null) throw new ArgumentNullException(nameof(didDocument));
        if (didDocument.AssertionMethod == null) throw new InvalidOperationException("There is no assertion method");
        if (string.IsNullOrWhiteSpace(verificationMethodId)) throw new ArgumentNullException(nameof(verificationMethodId));
        var handler = new JsonWebTokenHandler();
        var verificationMethod = didDocument.VerificationMethod.Single(m => m.Id == verificationMethodId);
        var asymKey = _verificationMethodEncoding.Decode(verificationMethod);
        var jsonWebToken = handler.ReadJsonWebToken(jwt);
        var content = System.Text.Encoding.UTF8.GetBytes($"{jsonWebToken.EncodedHeader}.{jsonWebToken.EncodedPayload}");
        var signature = Base64UrlEncoder.DecodeBytes(jsonWebToken.EncodedSignature);
        if (asymKey.CheckHash(content, signature, HashAlgorithmName.SHA256)) return true;
        return false;
    }
}
