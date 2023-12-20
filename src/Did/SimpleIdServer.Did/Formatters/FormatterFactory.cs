// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto.Multicodec;

namespace SimpleIdServer.Did.Formatters;

public class FormatterFactory
{
    public static IVerificationMethodFormatter BuildPublicKeyMultibase()
        => new PublicKeyMultibaseVerificationMethodFormatter(
                new MulticodecSerializer(
                    new IVerificationMethod[]
                    {
                        new Ed25519VerificationMethod(),
                        new Es256KVerificationMethod(),
                        new Es256VerificationMethod(),
                        new Es384VerificationMethod(),
                        new X25519VerificationMethod()
                    }
                )
            );

    public static IVerificationMethodFormatter BuildJWKVerificationMethod()
        => new JWKVerificationMethodFormatter(
                    new IVerificationMethod[]
                    {
                        new Ed25519VerificationMethod(),
                        new Es256KVerificationMethod(),
                        new Es256VerificationMethod(),
                        new Es384VerificationMethod(),
                        new X25519VerificationMethod()
                    });
}