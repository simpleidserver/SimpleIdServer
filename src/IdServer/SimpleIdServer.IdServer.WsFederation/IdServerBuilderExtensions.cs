// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.WsFederation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdServerBuilderExtensions
    {
        public static IdServerBuilder AddWsFederation(this IdServerBuilder idServerBuilder, Action<IdServerWsFederationOptions> callback = null)
        {
            if (callback == null) idServerBuilder.Services.Configure<IdServerWsFederationOptions>((o) => { });
            else idServerBuilder.Services.Configure(callback);
            return idServerBuilder;
        }

        public static IdServerBuilder AddWsFederationSigningCredentials(this IdServerBuilder idServerBuilder)
        {
            var certificate = GenerateSelfSignedCertificate();
            var securityKey = new X509SecurityKey(certificate, "wsFedKid");
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            idServerBuilder.KeyStore.Add(credentials);
            return idServerBuilder;
        }

        private static X509Certificate2 GenerateSelfSignedCertificate()
        {
            var subjectName = "Self-Signed-Cert-Example";
            var rsa = new RSACryptoServiceProvider(2048);
            var certRequest = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            var generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));
            var pfxGeneratedCert = new X509Certificate2(generatedCert.Export(X509ContentType.Pfx));
            return pfxGeneratedCert;
        }
    }
}
