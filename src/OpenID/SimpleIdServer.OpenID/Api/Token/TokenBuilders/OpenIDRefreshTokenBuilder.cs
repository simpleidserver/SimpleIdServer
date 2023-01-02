// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Options;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class OpenIDRefreshTokenBuilder : RefreshTokenBuilder
    {
        private readonly OAuthHostOptions _options;

        public OpenIDRefreshTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IOptions<OAuthHostOptions> options) : base(grantedTokenHelper, options) 
        {
            _options = options.Value;
        }

        public override async Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var dic = new JsonObject();
            if (handlerContext.Request.RequestData != null)
            {
                foreach (var record in handlerContext.Request.RequestData)
                {
                    dic.Add(record.Key, record.Value);
                }
            }

            if (handlerContext.User != null)
                dic.Add(JwtRegisteredClaimNames.Sub, handlerContext.User.Id);

            var authorizationCode = string.Empty;
            handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode);
            var refreshToken = await GrantedTokenHelper.AddRefreshToken(handlerContext.Client.ClientId, authorizationCode, dic, handlerContext.Client.RefreshTokenExpirationTimeInSeconds ?? _options.DefaultRefreshTokenExpirationTimeInSeconds, cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
        }
    }
}
