// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public interface IVerificationMethod
{
    string MulticodecHexValue { get; }
    int KeySize { get; }
    string Kty { get; }
    string CrvOrSize { get; }
    IAsymmetricKey Build(byte[] payload);
    IAsymmetricKey Build(JsonWebKey jwk);
}
