// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Helpers;

public static class HashHelper
{
    private static Dictionary<HashAlgorithmName, HashAlgorithm> _nameToAlgs = new Dictionary<HashAlgorithmName, HashAlgorithm>
    {
        { HashAlgorithmName.SHA256, SHA256.Create() }
    };

    public static byte[] Hash(byte[] content, HashAlgorithmName alg)
    {
        var algorithm = _nameToAlgs[alg];
        return algorithm.ComputeHash(content);
    }
}
