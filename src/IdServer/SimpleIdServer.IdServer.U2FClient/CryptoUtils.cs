// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fido2NetLib.Objects;
using System.Security.Cryptography;

namespace SimpleIdServer.IdServer.U2FClient
{
    public static class CryptoUtils
    {
        public static HashAlgorithmName HashAlgFromCOSEAlg(COSE.Algorithm alg)
        {
            return alg switch
            {
                COSE.Algorithm.RS1 => HashAlgorithmName.SHA1,
                COSE.Algorithm.ES256 => HashAlgorithmName.SHA256,
                COSE.Algorithm.ES384 => HashAlgorithmName.SHA384,
                COSE.Algorithm.ES512 => HashAlgorithmName.SHA512,
                COSE.Algorithm.PS256 => HashAlgorithmName.SHA256,
                COSE.Algorithm.PS384 => HashAlgorithmName.SHA384,
                COSE.Algorithm.PS512 => HashAlgorithmName.SHA512,
                COSE.Algorithm.RS256 => HashAlgorithmName.SHA256,
                COSE.Algorithm.RS384 => HashAlgorithmName.SHA384,
                COSE.Algorithm.RS512 => HashAlgorithmName.SHA512,
                COSE.Algorithm.ES256K => HashAlgorithmName.SHA256,
                (COSE.Algorithm)4 => HashAlgorithmName.SHA1,
                (COSE.Algorithm)11 => HashAlgorithmName.SHA256,
                (COSE.Algorithm)12 => HashAlgorithmName.SHA384,
                (COSE.Algorithm)13 => HashAlgorithmName.SHA512,
                COSE.Algorithm.EdDSA => HashAlgorithmName.SHA512
            };
        }
    }
}
