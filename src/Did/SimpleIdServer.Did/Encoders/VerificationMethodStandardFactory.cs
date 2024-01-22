// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Did.Encoders;

public class VerificationMethodStandardFactory
{
    public static IEnumerable<IVerificationMethodStandard> GetAll() =>
        new List<IVerificationMethodStandard>
        {
            new EcdsaSecp256k1RecoveryMethod2020Standard(),
            new EcdsaSecp256k1VerificationKey2019Standard(),
            new Ed25519VerificationKey2018Standard(),
            new Ed25519VerificationKey2020Standard(),
            new JsonWebKey2020Standard(),
            new X25519KeyAgreementKey2019Standard(),
            new X25519KeyAgreementKey2020Standard()
        };
}
