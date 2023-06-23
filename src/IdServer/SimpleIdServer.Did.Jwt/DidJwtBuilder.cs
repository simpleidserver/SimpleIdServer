// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System.Linq;

namespace SimpleIdServer.Did.Jwt
{
    public static class DidJwtBuilder
    {
        public static string GenerateToken(SecurityTokenDescriptor securityTokenDescriptor, IdentityDocument identityDocument, string privateKeyHex, string verificationMethodId = null) => GenerateToken(securityTokenDescriptor, identityDocument, privateKeyHex.HexToByteArray(), verificationMethodId);

        public static string GenerateToken(SecurityTokenDescriptor securityTokenDescriptor, IdentityDocument identityDocument, byte[] privateKeyPayload, string verificationMethodId =null)
        {
            var verificationMethod = identityDocument.VerificationMethod.First();
            if (!string.IsNullOrEmpty(verificationMethodId)) verificationMethod = identityDocument.VerificationMethod.First(m => m.Id == verificationMethodId);
            var alg = Constants.SupportedPublicKeyTypes.First(kvp => kvp.Value.Contains(verificationMethod.Type)).Key;
            var signatureKey = SignatureKeyFactory.Build(verificationMethod, alg, privateKeyPayload);
            var securityKey = new DidSecurityKey(signatureKey);
            securityTokenDescriptor.SigningCredentials = new SigningCredentials(securityKey, alg);
            var handler = new JsonWebTokenHandler();
            var result = handler.CreateToken(securityTokenDescriptor);
            securityKey.Dispose();
            return result;
        }
    }
}
