// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public class ECDSAP256SignHandler : ECDSASignHandler
    {
        public static string ALG_NAME = "ES256";
        public override string AlgName => ALG_NAME;
        public override CngAlgorithm CngHashAlg => CngAlgorithm.ECDsaP256;
        public override HashAlgorithmName HashAlg => HashAlgorithmName.SHA256;
    }
}