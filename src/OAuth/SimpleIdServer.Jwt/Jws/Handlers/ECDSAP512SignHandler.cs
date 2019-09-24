// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public class ECDSAP512SignHandler : ECDSASignHandler
    {
        public static string ALG_NAME = "ES512";
        public override string AlgName => ALG_NAME;
        public override CngAlgorithm CngHashAlg => CngAlgorithm.ECDiffieHellmanP521;
        public override HashAlgorithmName HashAlg => HashAlgorithmName.SHA512;
    }
}