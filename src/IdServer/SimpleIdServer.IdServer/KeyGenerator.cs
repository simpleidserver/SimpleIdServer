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
        public static SerializedFileKey GenerateRSASigningCredentials(Realm realm, string keyid = "keyId", string alg = SecurityAlgorithms.RsaSha256)
        {
            var sig = ClientKeyGenerator.GenerateRSASignatureKey(keyid, alg);
            var pem = PemConverter.ConvertFromSecurityKey(sig.Key);
            var result = new SerializedFileKey
            {
                Id = Guid.NewGuid().ToString(),
                Alg = alg,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                KeyId = sig.Kid,
                PrivateKeyPem = pem.PrivateKey,
                PublicKeyPem = pem.PublicKey,
                Usage = Constants.JWKUsages.Sig,
                IsSymmetric = false
            };
            result.Realms.Add(realm);
            return result;
        }

        public static SerializedFileKey GenerateECDSASigningCredentials(Realm realm, string keyid = "keyId", string alg = SecurityAlgorithms.EcdsaSha256)
        {
            var sig = ClientKeyGenerator.GenerateECDsaSignatureKey(keyid, alg);
            var pem = PemConverter.ConvertFromSecurityKey(sig.Key);
            var result = new SerializedFileKey
            {
                Id = Guid.NewGuid().ToString(),
                Alg = alg,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                KeyId = sig.Kid,
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

        public static (X509Certificate2, RSA) GenerateCertificateAuthority(string subjectName, int nbValidDays = 365)
        {
            var parent = RSA.Create(2048);
            CertificateRequest parentReq = new CertificateRequest(
                subjectName,
                parent,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            parentReq.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, false, 0, true));

            parentReq.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(parentReq.PublicKey, false));
            return (parentReq.CreateSelfSigned(
                    DateTimeOffset.UtcNow.AddDays(-1),
                    DateTimeOffset.UtcNow.AddDays(nbValidDays)), parent);
        }

        public static PemResult GenerateClientCertificate(X509Certificate2 ca, string subjectName, int nbValidDays)
        {
            using (var rsa = RSA.Create(2048))
            {
                CertificateRequest req = new CertificateRequest(
                    subjectName,
                    rsa,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);

                req.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(false, false, 0, false));

                req.CertificateExtensions.Add(
                    new X509KeyUsageExtension(
                        X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DataEncipherment,
                        false));

                req.CertificateExtensions.Add(
                    new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

                using (X509Certificate2 cert = req.Create(
                    ca,
                    DateTimeOffset.UtcNow.AddDays(-1),
                    DateTimeOffset.UtcNow.AddDays(nbValidDays),
                    new byte[] { 1, 2, 3, 4 }))
                {
                    var privatePem = new string(PemEncoding.Write("PRIVATE KEY", rsa.ExportPkcs8PrivateKey()));
                    return new PemResult(cert.ExportCertificatePem(), privatePem);
                }
            }
        }
    }
}
