// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public class QueryResponseModeHandler : IOAuthResponseModeHandler
    {
        public static string NAME = "query";
        public string ResponseMode => NAME;

        public void Handle(HandlerContext context, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var url = QueryHelpers.AddQueryString(authorizationResponse.RedirectUrl, authorizationResponse.QueryParameters);
            httpContext.Response.Redirect(url);
        }
    }
}
