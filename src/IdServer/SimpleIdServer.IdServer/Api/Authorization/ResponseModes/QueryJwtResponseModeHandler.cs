// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Jwt;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public class QueryJwtResponseModeHandler : BaseResponseModeHandler, IOAuthResponseModeHandler
    {
        public QueryJwtResponseModeHandler(IJwtBuilder jwtBuilder) : base(jwtBuilder)
        {
        }

        public static string NAME = "query.jwt";
        public string ResponseMode => NAME;

        public void Handle(HandlerContext context, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var jwt = BuildJWT(authorizationResponse, context);
            var dic = new Dictionary<string, string>
            {
                { AuthorizationResponseParameters.Response, jwt }
            };
            var url = QueryHelpers.AddQueryString(authorizationResponse.RedirectUrl, dic);
            httpContext.Response.Redirect(url);
        }
    }
}
