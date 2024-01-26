// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto
{
    public class RSASignatureKey : IAsymmetricKey
    {
        private RSA _rsa;

        private RSASignatureKey() 
        {
            _rsa = RSA.Create(2048);
        }

        public string Kty => Constants.StandardKty.RSA;

        public string CrvOrSize => Constants.StandardCrvOrSize.RSA2048;

        public static RSASignatureKey Generate()
        {
            return new RSASignatureKey();
        }

        public static RSASignatureKey From(byte[] publicKey, byte[] privateKey)
        {
            var result = new RSASignatureKey();
            result.Import(publicKey, privateKey);
            return result;
        }

        public static RSASignatureKey From(JsonWebKey publicJwk, JsonWebKey privateJwk)
        {
            var result = new RSASignatureKey();
            result.Import(publicJwk, privateJwk);
            return result;
        }

        public JsonWebKey GetPublicJwk()
        {
            var parameters = _rsa.ExportParameters(false);
            var result = new JsonWebKey
            {
                N = Base64UrlEncoder.Encode(parameters.Modulus),
                E = Base64UrlEncoder.Encode(parameters.Exponent)
            };
            return result;
        }

        public JsonWebKey GetPrivateJwk()
        {
            var parameters = _rsa.ExportParameters(true);
            var result = new JsonWebKey
            {
                P = Base64UrlEncoder.Encode(parameters.P),
                Q = Base64UrlEncoder.Encode(parameters.Q),
                D = Base64UrlEncoder.Encode(parameters.D),
                DQ = Base64UrlEncoder.Encode(parameters.DQ),
                DP = Base64UrlEncoder.Encode(parameters.DP),
                QI = Base64UrlEncoder.Encode(parameters.InverseQ)
            };
            return result;
        }

        public byte[] GetPublicKey(bool compressed = false)
        {
            return _rsa.ExportRSAPublicKey();
        }

        public byte[] GetPrivateKey()
        {
            return _rsa.ExportRSAPrivateKey();
        }

        public void Import(byte[] publicKey, byte[] privateKey)
        {
            if(publicKey != null)
            {
                _rsa.ImportRSAPublicKey(publicKey, out int nb);
            }

            if(privateKey != null)
            {
                _rsa.ImportRSAPrivateKey(privateKey, out int nb);
            }
        }

        public void Import(JsonWebKey publicJwk, JsonWebKey privateJwk)
        {
            if (publicJwk == null) throw new ArgumentNullException(nameof(publicJwk));
            if (publicJwk.N == null ||
                publicJwk.E == null) throw new ArgumentException("There is no public key");
            var rsaParameters = new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(publicJwk.N),
                Exponent = Base64UrlEncoder.DecodeBytes(publicJwk.E)
            };
            if(privateJwk != null)
            {
                if (privateJwk.P == null ||
                    privateJwk.Q == null ||
                    privateJwk.D == null ||
                    privateJwk.DQ == null ||
                    privateJwk.DP == null ||
                    privateJwk.QI == null)
                    throw new ArgumentException("There is no private key");
                rsaParameters.P = Base64UrlEncoder.DecodeBytes(privateJwk.P);
                rsaParameters.Q = Base64UrlEncoder.DecodeBytes(privateJwk.Q);
                rsaParameters.D = Base64UrlEncoder.DecodeBytes(privateJwk.D);
                rsaParameters.DQ = Base64UrlEncoder.DecodeBytes(privateJwk.DQ);
                rsaParameters.DP = Base64UrlEncoder.DecodeBytes(privateJwk.DP);
                rsaParameters.InverseQ = Base64UrlEncoder.DecodeBytes(privateJwk.QI);
            }

            _rsa.ImportParameters(rsaParameters);
        }

        public bool CheckHash(byte[] payload, byte[] signaturePayload, HashAlgorithmName? alg = null)
        {
            return _rsa.VerifyHash(payload, signaturePayload, alg.Value, RSASignaturePadding.Pkcs1);
        }

        public byte[] SignHash(byte[] content, HashAlgorithmName alg)
        {
            return _rsa.SignHash(content, alg, RSASignaturePadding.Pkcs1);
        }

        public SigningCredentials BuildSigningCredentials()
        {
            throw new NotImplementedException();
        }
    }
}
