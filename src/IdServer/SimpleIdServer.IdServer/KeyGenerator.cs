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

        public static X509Certificate2 GenerateRootAuthority(string subjectName = "simpleIdServer")
        {
            var rsa = RSA.Create();
            var parentReq = new CertificateRequest(
                $"CN={subjectName}",
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
            parentReq.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            parentReq.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(parentReq.PublicKey, false));
            return parentReq.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-45), DateTimeOffset.UtcNow.AddDays(365));
        }

        public static X509Certificate2 GenerateSelfSignedCertificate(X509Certificate2 parentCertificate, string subjectName)
        {
            var rsa = RSA.Create();
            var certRequest = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);            
            certRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation,false));
            certRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.8") }, true));
            certRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certRequest.PublicKey, false));
            var generatedCert = certRequest.Create(parentCertificate, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(10), new byte[] { 1, 2, 3, 4 });
            generatedCert = RSACertificateExtensions.CopyWithPrivateKey(generatedCert, rsa);
            return generatedCert;        
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
