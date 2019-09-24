// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseModes
{
    public class FragmentResponseModeHandler : IOAuthResponseModeHandler
    {
        public string ResponseMode => "fragment";

        public void Handle(RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var queryBuilder = new QueryBuilder(authorizationResponse.QueryParameters);
            httpContext.Response.Redirect($"{authorizationResponse.RedirectUrl}#{queryBuilder.ToQueryString()}");
        }
    }
}
