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
    public class VpJwtBuilder
    {
        public static string GenerateToken(IdentityDocument identityDocument, VerifiablePresentation verifiablePresentation, string privateHex, string verificationMethodId = null) => GenerateToken(identityDocument, verifiablePresentation, privateHex.HexToByteArray(), verificationMethodId);

        public static string GenerateToken(IdentityDocument identityDocument, VerifiablePresentation verifiablePresentation, byte[] privatePayload, string verificationMethodId = null)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor();
            var credentials = new List<string>();
            foreach(var cred in verifiablePresentation.VerifiableCredentials)
                credentials.Add(VcJwtBuilder.GenerateToken(identityDocument, cred, privatePayload, verificationMethodId));
            verifiablePresentation.VerifiableCredential = credentials;
            securityTokenDescriptor.Claims = new Dictionary<string, object>
            {
                { "iss", identityDocument.Id },
                { Constants.StandardClaims.Vp, verifiablePresentation.SerializeToDic() }
            };
            return DidJwtBuilder.GenerateToken(securityTokenDescriptor, identityDocument, privatePayload, verificationMethodId);
        }
    }
}