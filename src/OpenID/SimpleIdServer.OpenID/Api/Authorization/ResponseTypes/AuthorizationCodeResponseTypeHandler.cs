// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
            var dic = new Dictionary<string, object>
            {
                { UserClaims.Subject, context.User.Id }
            };
            if (context.Request.AuthDateTime != null)
            {
                dic.Add(OAuthClaims.AuthenticationTime, context.Request.AuthDateTime.Value.ConvertToUnixTimestamp());
            }

            if (context.Request.QueryParameters.ContainsKey(DTOs.AuthorizationRequestParameters.Claims))
            {
                dic.Add(Jwt.Constants.OAuthClaims.Claims, context.Request.QueryParameters[DTOs.AuthorizationRequestParameters.Claims]);
            }

            var scopes = context.Request.QueryParameters.GetScopesFromAuthorizationRequest();
            var jwsPayload = _grantedTokenHelper.BuildAccessToken(
                    new[] { context.Client.ClientId },
                    scopes, context.Request.IssuerName, context.Client.TokenExpirationTimeInSeconds
            );
            foreach(var kvp in dic)
            {
                jwsPayload.Add(kvp.Key, kvp.Value);
            }

            var authCode = _grantedTokenHelper.BuildAuthorizationCode(jwsPayload);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
            var isScopeCOntainsOfflineAccess = scopes.Contains(DTOs.OpenIDScopes.OfflineAccess);
            if (isScopeCOntainsOfflineAccess)
            {
                _tokenBuilders.First(t => t.Name == SimpleIdServer.OAuth.DTOs.TokenResponseParameters.RefreshToken).Build(context.Request.QueryParameters.GetScopesFromAuthorizationRequest(), context, dic);
            }
        }
    }
}