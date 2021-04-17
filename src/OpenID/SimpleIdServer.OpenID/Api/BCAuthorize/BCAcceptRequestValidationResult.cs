// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public class BCAcceptRequestValidationResult
    {
        public BCAcceptRequestValidationResult(OAuthUser user, Domains.BCAuthorize authorize)
        {
            User = user;
            Authorize = authorize;
        }

        public OAuthUser User { get; set; }
        public Domains.BCAuthorize Authorize { get; set; }
    }
}
