// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Ed25519VerificationMethod : IVerificationMethod
{
    public string MulticodecHexValue => MULTICODES_HEX_VALUE;

    public const string MULTICODES_HEX_VALUE = "0xed";

    public string Name => Constants.SupportedSignatureKeyAlgs.Ed25519;

    public int KeySize => 32;

    public ISignatureKey Build(byte[] payload) 
        => Ed25519SignatureKey.From(payload, null);
}