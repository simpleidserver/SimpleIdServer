// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Authorization.ResponseTypes
{
    public class AuthorizationCodeResponseTypeHandler : IResponseTypeHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;
        private readonly OAuthHostOptions _options;

        public AuthorizationCodeResponseTypeHandler(IGrantedTokenHelper grantedTokenHelper, IEnumerable<ITokenBuilder>  tokenBuilders, IOptions<OAuthHostOptions> options)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _tokenBuilders = tokenBuilders;
            _options = options.Value;
        }

        public string GrantType => AuthorizationCodeHandler.GRANT_TYPE;
        public string ResponseType => "code";
        public int Order => 1;

        public async Task Enrich(HandlerContext context, CancellationToken cancellationToken)
        {
            var dic = new JsonObject
            {  
                [JwtRegisteredClaimNames.Sub] = context.User.Id
            };

            var activeSession = context.User.ActiveSession;
            if (activeSession != null)
                dic.Add(JwtRegisteredClaimNames.AuthTime, activeSession.AuthenticationDateTime.ConvertToUnixTimestamp());

            foreach(var record in context.Request.RequestData)
                dic.Add(record.Key, record.Value);

            var authCode = await _grantedTokenHelper.AddAuthorizationCode(dic, _options.AuthorizationCodeExpirationInSeconds, cancellationToken);
            context.Response.Add(AuthorizationResponseParameters.Code, authCode);
            var isScopeContainsOfflineAccess = context.Request.RequestData.GetScopesFromAuthorizationRequest().Contains(SIDOpenIdConstants.StandardScopes.OfflineAccessScope.Name);
            if (isScopeContainsOfflineAccess)
            {
                await _tokenBuilders.First(t => t.Name == TokenResponseParameters.RefreshToken).Build(context.Request.RequestData.GetScopesFromAuthorizationRequest(), context, cancellationToken);
            }
        }
    }
}