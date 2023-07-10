// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.DPoP
{
    public class DPoPGenerationResult
    {
        public DPoPGenerationResult(JsonWebKey publicKey, string token)
        {
            PublicKey= publicKey;
            Token= token;
        }

        public JsonWebKey PublicKey { get; private set; }
        public string Token {  get; private set; }
    }
}
