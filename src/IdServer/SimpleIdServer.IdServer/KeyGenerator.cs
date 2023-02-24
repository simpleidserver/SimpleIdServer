// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer
{
    public static class KeyGenerator
    {
        public static X509Certificate2 GenerateSelfSignedCertificate()
        {
            var subjectName = "Self-Signed-Cert-Example";
            var rsa = new RSACryptoServiceProvider(2048, new CspParameters(24, "", Guid.NewGuid().ToString()));
            var certRequest = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            var generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));
            var pfxGeneratedCert = new X509Certificate2(generatedCert.Export(X509ContentType.Pfx), string.Empty, X509KeyStorageFlags.Exportable);
            return pfxGeneratedCert;
        }
    }
}
