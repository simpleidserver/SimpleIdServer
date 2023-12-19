// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class Es256KVerificationMethod : IVerificationMethod
{
    public string MulticodecHexValue => MULTICODES_HEX_VALUE;

    public const string MULTICODES_HEX_VALUE = "0xe7";

    public string Name => Constants.SupportedSignatureKeyAlgs.ES256K;

    public int KeySize => 33;

    public ISignatureKey Build(byte[] payload) 
        => ES256KSignatureKey.From(payload, null);
}
