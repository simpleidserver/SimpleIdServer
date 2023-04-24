// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System;

namespace SimpleIdServer.Did.Crypto
{
    public class SignatureKeyFactory
    {
        public static ISignatureKey Build(IdentityDocumentVerificationMethod validationMethod, string alg)
        {
            if (alg != SupportedSignatureKeyAlgs.ES256K && alg != SupportedSignatureKeyAlgs.Ed25519 && alg != SupportedSignatureKeyAlgs.ES256) throw new NotImplementedException("Only ES256K is supported");
            var publicKey = ExtractPublicKey(validationMethod);
            switch(alg)
            {
                case SupportedSignatureKeyAlgs.ES256K:
                    return new ES256KSignatureKey(publicKey);
                case SupportedSignatureKeyAlgs.Ed25519:
                    return new Ed25519SignatureKey(publicKey, null);
                case SupportedSignatureKeyAlgs.ES256:
                    return new ES256SignatureKey(publicKey, null);
                default:
                    throw new NotImplementedException($"{alg} is notsupported");
            }
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
