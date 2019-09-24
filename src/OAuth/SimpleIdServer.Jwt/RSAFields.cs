// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Jwt
{
    public static class RSAFields
    {
        public const string Modulus = "n";
        public const string Exponent = "e";
        public const string D = "d";
        public const string P = "p";
        public const string Q = "q";
        public const string DP = "dp";
        public const string DQ = "dq";
        public const string InverseQ = "qi";
        public static IEnumerable<string> PUBLIC_FIELDS = new[]
        {
            Modulus,
            Exponent
        };
        public static IEnumerable<string> PRIVATE_FIELDS = new[]
        {
            D,
            P,
            Q,
            DP,
            DQ,
            InverseQ
        };
    }
}
