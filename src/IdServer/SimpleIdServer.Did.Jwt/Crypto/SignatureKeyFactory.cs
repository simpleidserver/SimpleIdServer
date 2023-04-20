// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System;

namespace SimpleIdServer.Did.Jwt.Crypto
{
    public class SignatureKeyFactory
    {
        public static ISignatureKey Build(IdentityDocumentVerificationMethod validationMethod, string alg)
        {
            if (alg != SupportedJwtAlgs.ES256K) throw new NotImplementedException("Only ES256K is supported");
            var publicKey = ExtractPublicKey(validationMethod);
            return new ES256KSignatureKey(publicKey);
        }

        public static byte[] ExtractPublicKey(IdentityDocumentVerificationMethod validationMethod)
        {
            if (!string.IsNullOrWhiteSpace(validationMethod.PublicKeyBase64)) return Convert.FromBase64String(validationMethod.PublicKeyBase64);
            if (!string.IsNullOrWhiteSpace(validationMethod.PublicKeyHex)) return validationMethod.PublicKeyHex.HexToByteArray();
            if (validationMethod.PublicKeyJwk != null && validationMethod.PublicKeyJwk.ContainsKey("crv") && validationMethod.PublicKeyJwk.ContainsKey("x") && validationMethod.PublicKeyJwk.ContainsKey("y"))
            {
                var crv = validationMethod.PublicKeyJwk["crv"].GetValue<string>();
                var encodedX = validationMethod.PublicKeyJwk["x"].GetValue<string>();
                var encodedY = validationMethod.PublicKeyJwk["y"].GetValue<string>();
                if (crv != "secp256k1") throw new NotImplementedException($"crv {crv} is not supported");
                var namedCurve = SecNamedCurves.GetByName(crv);
                var x = new Org.BouncyCastle.Math.BigInteger(1, Base64UrlEncoder.DecodeBytes(encodedX));
                var y = new Org.BouncyCastle.Math.BigInteger(1, Base64UrlEncoder.DecodeBytes(encodedY));
                return namedCurve.Curve.CreatePoint(x, y).GetEncoded();
            }

            throw new NotImplementedException("Public key cannot be extracted");
        }
    }
}
