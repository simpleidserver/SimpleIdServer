// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Exceptions;

namespace SimpleIdServer.Uma.Exceptions
{
    public class UMAInvalidRequestException : OAuthException
    {
        public UMAInvalidRequestException(string message) : base(string.Empty, message)
        {
        }
    }
}
