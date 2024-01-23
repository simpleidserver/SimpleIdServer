// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Crypto.Multicodec;

public class MulticodecSerializerFactory
{
    public static IMulticodecSerializer Build() =>
        new MulticodecSerializer(
                    AllVerificationMethods
                );

    public static IVerificationMethod[] AllVerificationMethods = new IVerificationMethod[]
    {
        new Ed25519VerificationMethod(),
        new Es256KVerificationMethod(),
        new Es256VerificationMethod(),
        new Es384VerificationMethod(),
        new X25519VerificationMethod(),
        new RSAVerificationMethod()
    };
}
