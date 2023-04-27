// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Did.Key
{
    public static class Constants
    {
        public const string Type = "key";

        public static Dictionary<string, string> TypeToContextUrl = new Dictionary<string, string>
        {
            { SimpleIdServer.Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020, "https://w3id.org/security/suites/ed25519-2020/v1" },
            { SimpleIdServer.Did.Constants.VerificationMethodTypes.JsonWebKey2020, "https://w3id.org/security/suites/jws-2020/v1" }
        };

        public static class AdditionalVerificationMethodFields
        {
            public const string PublicKeyMultibase = "publicKeyMultibase";
        }
    }
}
