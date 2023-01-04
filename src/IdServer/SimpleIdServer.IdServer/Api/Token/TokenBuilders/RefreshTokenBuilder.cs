// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
{
    public class RefreshTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IdServerHostOptions _options;

        public RefreshTokenBuilder(IGrantedTokenHelper grantedTokenHelper, IOptions<IdServerHostOptions> options)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _options = options.Value;
        }

        protected IGrantedTokenHelper GrantedTokenHelper => _grantedTokenHelper;
        public string Name => TokenResponseParameters.RefreshToken;

        public virtual Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            return Build(scopes, new Dictionary<string, object>(), handlerContext, cancellationToken);
        }

        public virtual async Task Build(IEnumerable<string> scopes, Dictionary<string, object> claims, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var dic = new JsonObject();
            if (handlerContext.Request.RequestData != null)
                foreach (var record in handlerContext.Request.RequestData)
                    dic.Add(record.Key, record.Value.GetValue<string>());

            if (handlerContext.User != null)
                dic.Add(JwtRegisteredClaimNames.Sub, handlerContext.User.Id);

            var authorizationCode = string.Empty;
            handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode);
            var refreshToken = await GrantedTokenHelper.AddRefreshToken(handlerContext.Client.ClientId, authorizationCode, dic, handlerContext.Client.RefreshTokenExpirationTimeInSeconds ?? _options.DefaultRefreshTokenExpirationTimeInSeconds, cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
        }

        public virtual async Task Refresh(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var authorizationCode = string.Empty;
            handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode);
            var refreshToken = await  _grantedTokenHelper.AddRefreshToken(handlerContext.Client.ClientId, authorizationCode, previousQueryParameters, (handlerContext.Client.RefreshTokenExpirationTimeInSeconds ?? _options.DefaultRefreshTokenExpirationTimeInSeconds), cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
        }
    }
}
