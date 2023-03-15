// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Jwt;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public class FormPostJwtResponseModeHandler : BaseResponseModeHandler, IOAuthResponseModeHandler
    {
        public FormPostJwtResponseModeHandler(IJwtBuilder jwtBuilder) : base(jwtBuilder)
        {
        }

        public static string NAME = "form_post.jwt";
        public string ResponseMode => NAME;

        public void Handle(HandlerContext context, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var jwt = BuildJWT(authorizationResponse, context);
            var dic = new Dictionary<string, string>
            {
                { AuthorizationResponseParameters.Response, jwt }
            };
            var queryBuilder = new QueryBuilder(dic)
            {
                { AuthorizationResponseParameters.RedirectUrl, authorizationResponse.RedirectUrl }
            };
            var issuer = context.GetIssuer();
            var redirectUrl = $"{issuer}/{Constants.EndPoints.Form}{queryBuilder.ToQueryString()}";
            httpContext.Response.Redirect(redirectUrl);
        }
    }
}
