// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Formatters;

public interface IFormatterFactory
{
    IVerificationMethodFormatter ResolveFormatter(DidDocumentVerificationMethod verificationMethod);
}

public class FormatterFactory : IFormatterFactory
{
    private readonly IEnumerable<IVerificationMethodFormatter> _formatters;

    public FormatterFactory(IEnumerable<IVerificationMethodFormatter> formatters)
    {
        _formatters = formatters;
    }

    public FormatterFactory() : this(new IVerificationMethodFormatter[]
    {
        BuildEd25519VerificationKey2020Formatter(),
        BuildJsonWebKey2020Formatter(),
        BuildX25519KeyAgreementFormatter()
    })
    {

    }

    public IVerificationMethodFormatter ResolveFormatter(DidDocumentVerificationMethod verificationMethod)
    {
        var formatter = _formatters.Single(f => f.Type == verificationMethod.Type);
        return formatter;
    }

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
                        new X25519VerificationMethod(),
                        new RSAVerificationMethod()
                    });

    public static IVerificationMethodFormatter BuildX25519KeyAgreementFormatter()
        => new X25519KeyAgreementFormatter(
               MulticodecSerializerFactory.Build()
            );

    public static IVerificationMethodFormatter BuildEcdsaSecp256k1VerificationKey2019Formatter()
        => new EcdsaSecp256k1VerificationKey2019Formatter();
}