// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    public class JwtResponseModeHandler : BaseResponseModeHandler, IOAuthResponseModeHandler
    {
        public JwtResponseModeHandler(IJwtBuilder jwtBuilder) : base(jwtBuilder)
        {
        }


        public static string Name = "jwt";
        public string ResponseMode => Name;

        public void Handle(HandlerContext context, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var queryParams = context.Request.RequestData;
            var responseTypes = queryParams.GetResponseTypesFromAuthorizationRequest();
            // https://openid.net/specs/oauth-v2-jarm-final.html#section-2.3.4
            // Choose response mode.
            var responseMode = FragmentJwtResponseModeHandler.NAME;
            if (!responseTypes.Any() || (responseTypes.Count() == 1 && responseTypes.Contains(AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE)))
            {
                responseMode = QueryJwtResponseModeHandler.NAME;
            }

            var jwt = BuildJWT(authorizationResponse, context);
            var dic = new Dictionary<string, string>
            {
                { AuthorizationResponseParameters.Response, jwt }
            };
            if(responseMode == FragmentJwtResponseModeHandler.NAME)
            {
                var redirectUrl = $"{authorizationResponse.RedirectUrl}#{new QueryBuilder(dic).ToQueryString().ToString().TrimStart('?')}";
                httpContext.Response.Redirect(redirectUrl);
            }

            var url = QueryHelpers.AddQueryString(authorizationResponse.RedirectUrl, dic);
            httpContext.Response.Redirect(url);
        }
    }
}
