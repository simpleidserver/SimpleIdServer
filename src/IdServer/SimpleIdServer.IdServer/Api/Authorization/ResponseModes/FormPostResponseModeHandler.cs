// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseModes
{
    /// <summary>
    /// Implementation https://openid.net/specs/oauth-v2-form-post-response-mode-1_0.html
    /// </summary>
    public class FormPostResponseModeHandler : IOAuthResponseModeHandler
    {
        public const string NAME = "form_post";
        public string ResponseMode => NAME;

        public void Handle(HandlerContext context, RedirectURLAuthorizationResponse authorizationResponse, HttpContext httpContext)
        {
            var queryBuilder = new QueryBuilder(authorizationResponse.QueryParameters)
            {
                { "redirect_url", authorizationResponse.RedirectUrl }
            };
            var issuer = context.Request.IssuerName;
            var redirectUrl = $"{issuer}/{Constants.EndPoints.Form}{queryBuilder.ToQueryString()}";
            httpContext.Response.Redirect(redirectUrl);            
        }
    }
}