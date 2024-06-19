// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Encoders;

namespace SimpleIdServer.Did.Key;

public class DidKeyOptions
{
    public string PublicKeyFormat { get; set; }
    public bool EnableEncryptionKeyDerivation { get; set; } = false;
}
