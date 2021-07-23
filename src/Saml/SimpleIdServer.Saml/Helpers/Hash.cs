// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Saml.Helpers
{
    public static class Hash
    {
        public static byte[] Compute(string str, SignatureAlgorithms sigAlg)
        {
            var hash = Constants.MappingSignatureAlgToHash[sigAlg];
            if (hash == HashAlgorithmName.SHA1)
            {
                using (var sha1 = SHA1.Create())
                {
                    return sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
                }
            }

            if (hash == HashAlgorithmName.SHA256)
            {
                using (var sha256 = SHA256.Create())
                {
                    return sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                }
            }

            return null;
        }
    }
}
