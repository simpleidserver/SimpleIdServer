// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.OAuth.Exceptions
{
    public class OAuthUserConsentRequiredException : OAuthException
    {
        public OAuthUserConsentRequiredException() : base(string.Empty, string.Empty) { }
    }
}
