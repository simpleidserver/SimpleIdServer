// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Exceptions
{
    public class OAuthLoginRequiredException : OAuthException
    {
        public OAuthLoginRequiredException() : base(string.Empty, string.Empty) { }

        public OAuthLoginRequiredException(string area, bool eraseCookie = false) : this()
        {
            Area = area;
        }

        public string Area { get; private set; }
        public bool EraseCookie { get; private set; }
    }
}
