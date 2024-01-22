// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Formatters;

namespace SimpleIdServer.Did.Ethr;

public class VerificationMethodTypeResolver
{
    public static string Resolve(
        string keyAlgorithm)
    {
        switch(keyAlgorithm)
        {
            case "Secp256k1":
                return EcdsaSecp256k1VerificationKey2019Formatter.TYPE;
            case "Ed25519":
                return Ed25519VerificationKey2018Formatter.TYPE;
            case "X25519":
                return X25519KeyAgreementKey2019Formatter.TYPE;
        }

        return null;
    }
}
