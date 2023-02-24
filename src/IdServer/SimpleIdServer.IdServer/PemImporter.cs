// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer
{
    public static class PemImporter
    {
        public static SecurityKey Import<T>(PemResult content, string keyId) where T : SecurityKey
        {
            if (typeof(T) == typeof(RsaSecurityKey))
                return ImportRSA(content, keyId);

            if (typeof(T) == typeof(X509SecurityKey))
                return ImportCertificate(content, keyId);

            if (typeof(T) == typeof(ECDsaSecurityKey))
                return ImportECDsa(content, keyId);

            throw new NotImplementedException();
        }

        public static RsaSecurityKey ImportRSA(PemResult content, string keyId)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(content.PrivateKey);
            rsa.ImportFromPem(content.PublicKey);
            return new RsaSecurityKey(rsa)
            {
                KeyId = keyId
            };
        }

        public static X509SecurityKey ImportCertificate(PemResult content, string keyId)
        {
            var certificate = X509Certificate2.CreateFromPem(content.PublicKey, content.PrivateKey);
            return new X509SecurityKey(certificate)
            {
                KeyId = keyId
            };
        }

        public static ECDsaSecurityKey ImportECDsa(PemResult content, string keyId)
        {
            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(content.PublicKey);
            ecdsa.ImportFromPem(content.PrivateKey);
            return new ECDsaSecurityKey(ecdsa)
            {
                KeyId = keyId
            };
        }
    }
}
