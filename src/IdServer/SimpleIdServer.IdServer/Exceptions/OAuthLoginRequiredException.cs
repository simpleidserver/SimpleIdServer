// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Exceptions
{
    public class OAuthLoginRequiredException : OAuthException
    {
        public OAuthLoginRequiredException() : base(string.Empty, string.Empty) { }

        public OAuthLoginRequiredException(bool eraseCookie = false) : this()
        {
            EraseCookie = eraseCookie;
        }

        public bool EraseCookie { get; private set; }
    }
}
