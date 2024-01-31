// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;

namespace SimpleIdServer.Did.Key;

public class DidKeyGenerator
{
    private readonly IMulticodecSerializer _serializer;

    public DidKeyGenerator(IMulticodecSerializer serializer)
    {
        _serializer = serializer;
    }

    public static DidKeyGenerator New()
    {
        var serializer = MulticodecSerializerFactory.Build();
        return new DidKeyGenerator(serializer);
    }

    public string Generate(IAsymmetricKey key)
    {
        var multicodec = _serializer.SerializePublicKey(key);
        return $"{Did.Constants.Scheme}:{Constants.Type}:{multicodec}";
    }

    public string GenerateRandom()
    {
        var ed25519 = Ed25519SignatureKey.Generate();
        return Generate(ed25519);
    }
}
