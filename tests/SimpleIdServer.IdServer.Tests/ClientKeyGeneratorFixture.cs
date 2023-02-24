// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace SimpleIdServer.IdServer.Tests
{
    public class ClientKeyGeneratorFixture
    {
        [Test]
        public void GenerateRSAClientSignatureKey()
        {
            var rsaSig = ClientKeyGenerator.GenerateRSASignatureKey("keyId");
            var rsaSigPem = PemConverter.ConvertFromSecurityKey(rsaSig.Key);
            var importedRsa = PemImporter.Import<RsaSecurityKey>(rsaSigPem, "keyId");
            Assert.NotNull(importedRsa);
        }

        [Test]
        public void GenerateCertificateClientSignatureKey()
        {
            var certificateSig = ClientKeyGenerator.GenerateX509CertificateSignatureKey("keyId");
            var certificateSigPem = PemConverter.ConvertFromSecurityKey(certificateSig.Key);
            var importedCertificate = PemImporter.Import<X509SecurityKey>(certificateSigPem, "keyId");
            Assert.NotNull(importedCertificate);
        }

        [Test]
        public void GenerateECDsaClientSignatureKey()
        {
            var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId");
            var ecdsaSigPem = PemConverter.ConvertFromSecurityKey(ecdsaSig.Key);
            var importedECDsa = PemImporter.Import<ECDsaSecurityKey>(ecdsaSigPem, "keyId");
            Assert.NotNull(importedECDsa);
        }
    }
}
