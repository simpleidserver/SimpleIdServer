// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Did.Encoders;

[Flags]
public enum SignatureKeyEncodingTypes
{
    BASE58 = 1,
    HEX = 2,
    MULTIBASE = 4,
    JWK = 8
}