// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.WsFederation
{
    public static class WsFederationKeyGenerator
    {
        public static SerializedFileKey GenerateWsFederationSigningCredentials(Domains.Realm realm)
        {
            var certificate = KeyGenerator.GenerateSelfSignedCertificate();
            var securityKey = new X509SecurityKey(certificate, "wsFedKid");
            var pem = PemConverter.ConvertFromX509SecurityKey(securityKey);
            var result = new SerializedFileKey
            {
                Alg = SecurityAlgorithms.RsaSha256,
                CreateDateTime= DateTime.UtcNow,
                UpdateDateTime= DateTime.UtcNow,
                Id = Guid.NewGuid().ToString(),
                KeyId = "wsFedKid",
                PrivateKeyPem = pem.PrivateKey,
                PublicKeyPem = pem.PublicKey,
                Usage = SimpleIdServer.IdServer.Constants.JWKUsages.Sig,
                IsSymmetric = false
            };
            result.Realms.Add(realm);
            return result;
        }
    }
}
