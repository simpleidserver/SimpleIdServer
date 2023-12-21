// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto.Multicodec;

namespace SimpleIdServer.Did.Formatters;

public class FormatterFactory
{
    public static IVerificationMethodFormatter BuildEd25519VerificationKey2020Formatter()
        => new Ed25519VerificationKey2020Formatter(
               MulticodecSerializerFactory.Build()
            );

    public static IVerificationMethodFormatter BuildJsonWebKey2020Formatter()
        => new JsonWebKey2020Formatter(
                    new IVerificationMethod[]
                    {
                        new Ed25519VerificationMethod(),
                        new Es256KVerificationMethod(),
                        new Es256VerificationMethod(),
                        new Es384VerificationMethod(),
                        new X25519VerificationMethod()
                    });

    public static IVerificationMethodFormatter BuildX25519KeyAgreementFormatter()
        => new X25519KeyAgreementFormatter(
               MulticodecSerializerFactory.Build()
            );
}