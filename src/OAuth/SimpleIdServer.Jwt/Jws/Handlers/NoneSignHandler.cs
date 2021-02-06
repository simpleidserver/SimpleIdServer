// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public class NoneSignHandler : ISignHandler
    {
        public string AlgName => ALG_NAME;

        public static string ALG_NAME = "none";

        public string Sign(string payload, JsonWebKey jsonWebKey)
        {
            return string.Empty;
        }

        public bool Verify(string payload, byte[] signature, JsonWebKey jsonWebKey)
        {
            return true;
        }
    }
}
