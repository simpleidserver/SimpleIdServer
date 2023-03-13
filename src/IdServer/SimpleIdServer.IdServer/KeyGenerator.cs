// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer
{
    public static class KeyGenerator
    {
        public static SerializedFileKey GenerateSigningCredentials(Realm realm)
        {
            var rsa = RSA.Create();
            var key = new RsaSecurityKey(rsa)
            {
                KeyId = "keyid"
            };
            var pem = PemConverter.ConvertFromSecurityKey(key);
            var result = new SerializedFileKey
            {
                Id = Guid.NewGuid().ToString(),
                Alg = SecurityAlgorithms.RsaSha256,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                KeyId = key.KeyId,
                PrivateKeyPem = pem.PrivateKey,
                PublicKeyPem = pem.PublicKey,
                Usage = Constants.JWKUsages.Sig,
                IsSymmetric = false
            };
            result.Realms.Add(realm);
            return result;
        }

        public static X509Certificate2 GenerateSelfSignedCertificate()
        {
            var subjectName = "Self-Signed-Cert-Example";
            var rsa = RSA.Create();
            var certRequest = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            var generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));
            return generatedCert;
        }
    }
}
