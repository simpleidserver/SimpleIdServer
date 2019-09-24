// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Jwt
{
    public static class ErrorMessages
    {
        public const string INVALID_JWS = "invalid jws";
        public const string INVALID_JWE = "invalid JWE";
        public const string BAD_JWS = "bad jws";
        public const string UNKNOWN_ALG = "unknown alg {0}";
        public const string UNKNOWN_ENC = "unknown enc {0}";
    }
}