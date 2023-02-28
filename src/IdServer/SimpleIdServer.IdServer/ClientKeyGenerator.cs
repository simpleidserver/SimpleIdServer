// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.IdServer
{
    public static class ClientKeyGenerator
    {
        /// <summary>
        /// Generate encryption key used to generate encrypted 'id_token'.
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="alg"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static EncryptingCredentials GenerateRSAEncryptionKey(string keyId, string alg = SecurityAlgorithms.RsaPKCS1, string enc = SecurityAlgorithms.Aes128CbcHmacSha256)
        {
            var rsa = RSA.Create();
            var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
            var credentials = new EncryptingCredentials(securityKey, alg, enc);
            return credentials;
        }

        public static EncryptingCredentials GenerateCertificateEncryptionKey(string keyId, string alg = SecurityAlgorithms.RsaPKCS1, string enc = SecurityAlgorithms.Aes128CbcHmacSha256)
        {
            var x509SecurityKey = new X509SecurityKey(KeyGenerator.GenerateSelfSignedCertificate(), keyId);
            return new EncryptingCredentials(x509SecurityKey, alg, enc);
        }

        public static EncryptingCredentials GenerateECDsaEncryptionKey(string keyId, string alg = SecurityAlgorithms.EcdhEs, string enc = SecurityAlgorithms.Aes128CbcHmacSha256)
        {
            if (alg != SecurityAlgorithms.EcdhEs && alg != SecurityAlgorithms.EcdhEsA128kw && alg != SecurityAlgorithms.EcdhEsA192kw && alg != SecurityAlgorithms.EcdhEsA256kw)
                throw new NotSupportedException($"algorithm '{alg}' is not supported");

            var ecdsaSecurityKey = new ECDsaSecurityKey(ECDsa.Create()) { KeyId = keyId }; ;
            return new EncryptingCredentials(ecdsaSecurityKey, alg, enc);
        }

        /// <summary>
        /// Generate signature key used to check 'request' object and used during the 'private_key_jwt' & 'client_secret_jwt' authentication flows.
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="alg"></param>
        public static SigningCredentials GenerateRSASignatureKey(string keyId, string alg = SecurityAlgorithms.RsaSha256)
        {
            var rsa = RSA.Create();
            var securityKey = new RsaSecurityKey(rsa) { KeyId = keyId };
            return new SigningCredentials(securityKey, alg);
        }

        public static SigningCredentials GenerateX509CertificateSignatureKey(string keyId, string alg = SecurityAlgorithms.RsaSha256)
        {
            var x509SecurityKey = new X509SecurityKey(KeyGenerator.GenerateSelfSignedCertificate(), keyId);
            return new SigningCredentials(x509SecurityKey, alg);
        }

        public static SigningCredentials GenerateECDsaSignatureKey(string keyId, string alg = SecurityAlgorithms.EcdsaSha256)
        {
            if (alg != SecurityAlgorithms.EcdsaSha256 && alg != SecurityAlgorithms.EcdsaSha384 && alg != SecurityAlgorithms.EcdsaSha512)
                throw new NotSupportedException($"algorithm '{alg}' is not supported");

            var curve = ECCurve.NamedCurves.nistP256;
            if(alg == SecurityAlgorithms.EcdsaSha384) curve = ECCurve.NamedCurves.nistP384;
            else curve = ECCurve.NamedCurves.nistP521;

            var ecdsaSecurityKey = new ECDsaSecurityKey(ECDsa.Create(curve)) { KeyId = keyId };
            return new SigningCredentials(ecdsaSecurityKey, alg);
        }
    }
}
