// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Exceptions
{
    public class OAuthDPoPRequiredException : OAuthException
    {
        public OAuthDPoPRequiredException(string nonce, string code, string message) : base(code, message)
        {
            Nonce = nonce;
        }

        public string Nonce { get; set; }
    }
}
