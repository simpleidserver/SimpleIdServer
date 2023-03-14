// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer
{
    public static class PemConverter
    {
        public static PemResult ConvertFromSecurityKey(SecurityKey key)
        {
            if (key is RsaSecurityKey rsaKey)
                return ConvertFromRSASecurityKey(rsaKey);

            if (key is X509SecurityKey x509Key)
                return ConvertFromX509SecurityKey(x509Key);

            if (key is ECDsaSecurityKey ecdsaKey)
                return ConvertFromECDsaSecurityKey(ecdsaKey);

            throw new NotImplementedException();
        }

        public static PemResult ConvertFromRSASecurityKey(RsaSecurityKey key)
        {
            var rsa = key.Rsa;
            var publicPem = rsa.ExportRSAPublicKeyPem();
            var privatePem = rsa.ExportRSAPrivateKeyPem();
            return new PemResult(publicPem, privatePem);
        }

        public static PemResult ConvertFromX509SecurityKey(X509SecurityKey key)
        {
            var certificate = key.Certificate;
            var publicPem = PemEncoding.Write("CERTIFICATE", certificate.RawData);
            var privatePem = string.Empty;
            if (certificate.GetRSAPrivateKey() is RSA rsaKey)
                privatePem = new string(PemEncoding.Write("PRIVATE KEY", rsaKey.ExportPkcs8PrivateKey()));
            else if (certificate.GetECDsaPrivateKey() is ECDsa ecdsaKey)
                privatePem = new string(PemEncoding.Write("PRIVATE KEY", ecdsaKey.ExportPkcs8PrivateKey()));
            else if (certificate.GetDSAPrivateKey() is DSA dsaKey)
                privatePem = new string(PemEncoding.Write("PRIVATE KEY", dsaKey.ExportPkcs8PrivateKey()));
            else
                throw new CryptographicException("Unknown certificate algorithm");
            return new PemResult(new string(publicPem), privatePem);
        }

        public static PemResult ConvertFromECDsaSecurityKey(ECDsaSecurityKey key)
        {
            var publicPem = key.ECDsa.ExportSubjectPublicKeyInfoPem();
            var privatePem = key.ECDsa.ExportECPrivateKeyPem();
            return new PemResult(publicPem, privatePem);
        }
    }

    public class PemResult
    {
        public PemResult(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }
    }
}
