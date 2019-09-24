// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Jwt
{
    public static class ECFields
    {
        public const string CURVE = "crv";
        public const string X = "x";
        public const string Y = "y";
        public const string D = "d";
        public static IEnumerable<string> PUBLIC_FIELDS = new[]
        {
            CURVE,
            X,
            D
        };
        public static IEnumerable<string> PRIVATE_FIELDS = new[]
        {
            D
        };
    }
}
