// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public class PS512SignHandler : PSSignHandler
    {
        public static string ALG_NAME = "PS512";
        public override string AlgName => ALG_NAME;
        public override HashAlgorithmName Alg => HashAlgorithmName.SHA512;
    }
}
