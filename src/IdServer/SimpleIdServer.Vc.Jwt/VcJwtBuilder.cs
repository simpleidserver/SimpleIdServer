// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Did.Models;
using SimpleIdServer.Vc.Models;
using System.Collections.Generic;

namespace SimpleIdServer.Vc.Jwt
{
    public static class VcJwtBuilder
    {
        public static string GenerateToken(IdentityDocument identityDocument, VerifiableCredential verifiableCredential, string privateHex, string verificationMethodId = null) => GenerateToken(identityDocument, verifiableCredential, privateHex.HexToByteArray(), verificationMethodId);

        public static string GenerateToken(IdentityDocument identityDocument, VerifiableCredential verifiableCredential, byte[] privatePayload, string verificationMethodId = null)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor();
            if (!string.IsNullOrWhiteSpace(verifiableCredential.Issuer)) securityTokenDescriptor.Issuer = verifiableCredential.Issuer;
            if (verifiableCredential.IssuanceDate != null) securityTokenDescriptor.IssuedAt = verifiableCredential.IssuanceDate;
            securityTokenDescriptor.Claims = new Dictionary<string, object>
            {
                { "sub", identityDocument.Id },
                { Constants.StandardClaims.Vc, verifiableCredential.SerializeToDic() }
            };
            return DidJwtBuilder.GenerateToken(securityTokenDescriptor, identityDocument, privatePayload, verificationMethodId);
        }
    }
}
