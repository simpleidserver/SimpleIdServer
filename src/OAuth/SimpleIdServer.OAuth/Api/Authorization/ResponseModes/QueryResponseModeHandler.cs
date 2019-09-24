// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseModes
{
    public class QueryResponseModeHandler : IOAuthResponseModeHandler
    {
        public string ResponseMode => "query";

        public void Handle(RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var url = QueryHelpers.AddQueryString(authorizationResponse.RedirectUrl, authorizationResponse.QueryParameters);
            httpContext.Response.Redirect(url);
        }
    }
}
