// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public interface IVerificationMethod
{
    string MulticodecHexValue { get; }
    int KeySize { get; }
    string Name { get; }
    ISignatureKey Build(byte[] payload);
}
