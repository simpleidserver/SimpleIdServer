// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public class FragmentResponseModeHandler : IOAuthResponseModeHandler
    {
        public static string NAME = "fragment";
        public string ResponseMode => NAME;

        public void Handle(HandlerContext context, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var queryBuilder = new QueryBuilder(authorizationResponse.QueryParameters);
            var redirectUrl = $"{authorizationResponse.RedirectUrl}#{queryBuilder.ToQueryString().ToString().TrimStart('?')}";
            httpContext.Response.Redirect(redirectUrl);
        }
    }
}