// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Exceptions
{
    public class OAuthExceptionBadRequestURIException : OAuthException
    {
        public OAuthExceptionBadRequestURIException(string redirectUri) : base(ErrorCodes.INVALID_REQUEST, string.Format(Global.BadRedirectUri, redirectUri)) { }
    }
}
