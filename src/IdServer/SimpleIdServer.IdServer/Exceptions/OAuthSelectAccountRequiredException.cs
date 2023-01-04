// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Exceptions
{
    public class OAuthSelectAccountRequiredException : OAuthException
    {
        public OAuthSelectAccountRequiredException() : base(string.Empty, string.Empty)
        {
        }
    }
}
