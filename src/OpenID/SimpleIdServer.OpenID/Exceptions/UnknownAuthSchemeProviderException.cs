// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Exceptions;

namespace SimpleIdServer.OpenID.Exceptions
{
    public class UnknownAuthSchemeProviderException : OAuthException
    {
        public UnknownAuthSchemeProviderException(string code, string message) : base(code, message) { }
    }
}
