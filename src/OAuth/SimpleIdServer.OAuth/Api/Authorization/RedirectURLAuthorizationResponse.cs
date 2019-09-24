// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Api.Authorization
{
    public class RedirectURLAuthorizationResponse : AuthorizationResponse
    {
        public RedirectURLAuthorizationResponse(string redirectUrl, Dictionary<string, string> queryParameters) : base(AuthorizationResponseTypes.RedirectUrl)
        {
            RedirectUrl = redirectUrl;
            QueryParameters = queryParameters;
        }

        public string RedirectUrl { get; private set; }
        public Dictionary<string, string> QueryParameters { get; private set; }
    }
}