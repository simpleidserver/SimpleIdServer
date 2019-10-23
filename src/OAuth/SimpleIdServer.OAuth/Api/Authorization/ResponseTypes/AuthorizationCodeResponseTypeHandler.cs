// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;

namespace SimpleIdServer.OAuth.Api.Authorization.ResponseTypes
{
    public class AuthorizationCodeResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;

        public AuthorizationCodeResponseTypeHandler(IGrantedTokenHelper grantedTokenHelper)
        {
            _grantedTokenHelper = grantedTokenHelper;
        }

        public string GrantType => AuthorizationCodeHandler.GRANT_TYPE;
        public string ResponseType => RESPONSE_TYPE;
        public int Order => 1;
        public static string RESPONSE_TYPE = "code";

        public void Enrich(HandlerContext context)
        {
            var dic = new JObject();
            foreach (var record in context.Request.QueryParameters)
            {
                dic.Add(record.Key, record.Value);
            }

            var authCode = _grantedTokenHelper.BuildAuthorizationCode(dic);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
        }
    }
}