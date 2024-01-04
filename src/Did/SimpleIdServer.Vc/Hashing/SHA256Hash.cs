// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Security.Cryptography;

namespace SimpleIdServer.Vc.Hashing;

public class SHA256Hash : IHashing
{
    public HashAlgorithmName Name => HashAlgorithmName.SHA256;

    public byte[] Hash(byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(data);
        }
    }
}
