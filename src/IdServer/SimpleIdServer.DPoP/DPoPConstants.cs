// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.DPoP
{
    public static class DPoPConstants
    {
        public const string DPoPTyp = "dpop+jwt";
        public const string Jwk = "jwk";

        public static class DPoPClaims
        {
            public const string Htm = "htm";
            public const string Nonce = "nonce";
            public const string Htu = "htu";
        }
    }
}
