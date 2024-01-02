// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Vc.Hashing;

public interface IHashing
{
    string Name { get; }
    byte[] Hash(byte[] data);
}
