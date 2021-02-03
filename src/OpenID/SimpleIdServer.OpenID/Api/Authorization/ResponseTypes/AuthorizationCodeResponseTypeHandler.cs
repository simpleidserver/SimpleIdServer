// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Linq;
using static SimpleIdServer.Jwt.Constants;

namespace SimpleIdServer.OpenID.Api.Authorization.ResponseTypes
{
    public class AuthorizationCodeResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public AuthorizationCodeResponseTypeHandler(IGrantedTokenHelper grantedTokenHelper, IEnumerable<ITokenBuilder>  tokenBuilders)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _tokenBuilders = tokenBuilders;
        }

        public string GrantType => AuthorizationCodeHandler.GRANT_TYPE;
        public string ResponseType => "code";
        public int Order => 1;

        public void Enrich(HandlerContext context)
        {
            var dic = new JObject
            {
                { UserClaims.Subject, context.User.Id }
            };

            if (context.User.AuthenticationTime != null)
            {
                dic.Add(OAuthClaims.AuthenticationTime, context.User.AuthenticationTime.Value.ConvertToUnixTimestamp());
            }

            foreach(var record in context.Request.Data)
            {
                dic.Add(record.Key, record.Value);
            }

            var authCode = _grantedTokenHelper.BuildAuthorizationCode(dic);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
            var isScopeCOntainsOfflineAccess = context.Request.Data.GetScopesFromAuthorizationRequest().Contains(SIDOpenIdConstants.StandardScopes.OfflineAccessScope.Name);
            if (isScopeCOntainsOfflineAccess)
            {
                _tokenBuilders.First(t => t.Name == TokenResponseParameters.RefreshToken).Build(context.Request.Data.GetScopesFromAuthorizationRequest(), context, dic);
            }
        }
    }
}