// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace SimpleIdServer.IdServer.Tests
{
    public class ClientKeyGeneratorFixture
    {
        [Test]
        [TestCase(SecurityAlgorithms.RsaSha256)]
        [TestCase(SecurityAlgorithms.RsaSha384)]
        [TestCase(SecurityAlgorithms.RsaSha512)]
        public void GenerateRSAClientSignatureKey(string alg)
        {
            var rsaSig = ClientKeyGenerator.GenerateRSASignatureKey("keyId", alg);
            var jwt = Sign(rsaSig);
            var rsaSigPem = PemConverter.ConvertFromSecurityKey(rsaSig.Key);
            var importedRsa = PemImporter.Import<RsaSecurityKey>(rsaSigPem, "keyId");
            var sigResult = CheckSignature(jwt, importedRsa);
            Assert.That(sigResult.IsValid);
        }

        [Test]
        [TestCase(SecurityAlgorithms.RsaSha256)]
        [TestCase(SecurityAlgorithms.RsaSha384)]
        [TestCase(SecurityAlgorithms.RsaSha512)]
        public void GenerateCertificateClientSignatureKey(string alg)
        {
            var certificateSig = ClientKeyGenerator.GenerateX509CertificateSignatureKey("keyId", alg);
            var jwt = Sign(certificateSig);
            var certificateSigPem = PemConverter.ConvertFromSecurityKey(certificateSig.Key);
            var importedCertificate = PemImporter.Import<X509SecurityKey>(certificateSigPem, "keyId");
            var sigResult = CheckSignature(jwt, importedCertificate);
            Assert.That(sigResult.IsValid);
        }

        [Test]
        [TestCase(SecurityAlgorithms.EcdsaSha256)]
        [TestCase(SecurityAlgorithms.EcdsaSha384)]
        [TestCase(SecurityAlgorithms.EcdsaSha512)]
        public void GenerateECDsaClientSignatureKey(string alg)
        {
            var ecdsaSig = ClientKeyGenerator.GenerateECDsaSignatureKey("keyId", alg);
            var jwt = Sign(ecdsaSig);
            var ecdsaSigPem = PemConverter.ConvertFromSecurityKey(ecdsaSig.Key);
            var importedECDsa = PemImporter.Import<ECDsaSecurityKey>(ecdsaSigPem, "keyId");
            var sigResult = CheckSignature(jwt, importedECDsa);
            Assert.That(sigResult.IsValid);
        }

        [Test]
        [TestCase(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256)]
        [TestCase(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384)]
        [TestCase(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512)]
        [TestCase(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256)]
        [TestCase(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384)]
        [TestCase(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512)]
        public void GenerateRSAClientEncryptionKey(string alg, string enc)
        {
            var rsaEnc = ClientKeyGenerator.GenerateRSAEncryptionKey("keyId", alg, enc);
            var encJwt = Encrypt(rsaEnc);
            var rsaEncPem = PemConverter.ConvertFromSecurityKey(rsaEnc.Key);
            var importedRsa = PemImporter.Import<RsaSecurityKey>(rsaEncPem, "keyId");
            var decrypted = Decrypt(encJwt, importedRsa);
            Assert.That(decrypted == "JWT");
        }

        [Test]
        [TestCase(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256)]
        [TestCase(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes192CbcHmacSha384)]
        [TestCase(SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes256CbcHmacSha512)]
        [TestCase(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes128CbcHmacSha256)]
        [TestCase(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes192CbcHmacSha384)]
        [TestCase(SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512)]
        public void GenerateCertificateClientEncryptionKey(string alg, string enc)
        {
            var rsaEnc = ClientKeyGenerator.GenerateCertificateEncryptionKey("keyId", alg, enc);
            var encJwt = Encrypt(rsaEnc);
            var rsaEncPem = PemConverter.ConvertFromSecurityKey(rsaEnc.Key);
            var importedRsa = PemImporter.Import<X509SecurityKey>(rsaEncPem, "keyId");
            var decrypted = Decrypt(encJwt, importedRsa);
            Assert.That(decrypted == "JWT");
        }

        private static string Sign(SigningCredentials signingCredentials)
        {
            var handler = new JsonWebTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    { "claim", "value" }
                },
                SigningCredentials = signingCredentials
            };
            return handler.CreateToken(descriptor);
        }

        private static TokenValidationResult CheckSignature(string jwt, SecurityKey securityKey)
        {
            return new JsonWebTokenHandler().ValidateToken(jwt, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                IssuerSigningKey = securityKey
            });
        }

        private static string Encrypt(EncryptingCredentials encCredentials)
        {
            var handler = new JsonWebTokenHandler();
            return handler.EncryptToken("JWT", encCredentials);
        }

        private static string Decrypt(string jwt, SecurityKey encSecurityKey)
        {
            var handler = new JsonWebTokenHandler();
            var jsonWebToken = handler.ReadJsonWebToken(jwt);
            return handler.DecryptToken(jsonWebToken, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                TokenDecryptionKey = encSecurityKey
            });
        }
    }
}
