﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Helpers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public class RefreshTokenBuilder : ITokenBuilder
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;

        public RefreshTokenBuilder(IGrantedTokenHelper grantedTokenHelper)
        {
            _grantedTokenHelper = grantedTokenHelper;
        }

        protected IGrantedTokenHelper GrantedTokenHelper => _grantedTokenHelper;
        public string Name => TokenResponseParameters.RefreshToken;

        public virtual Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            return Build(scopes, new JObject(), handlerContext, cancellationToken);
        }

        public virtual async Task Build(IEnumerable<string> scopes, JObject jObj, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var dic = new JObject();
            if (handlerContext.Request.RequestData != null)
            {
                foreach (var record in handlerContext.Request.RequestData)
                {
                    dic.Add(record.Key, record.Value);
                }
            }

            var authorizationCode = string.Empty;
            if (!handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode))
            {
                authorizationCode = handlerContext.Request.RequestData.GetAuthorizationCode();
            }

            var refreshToken = await _grantedTokenHelper.AddRefreshToken(handlerContext.Client.ClientId, authorizationCode, dic, handlerContext.Client.RefreshTokenExpirationTimeInSeconds, cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
        }

        public virtual async Task Refresh(JObject previousQueryParameters, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var authorizationCode = string.Empty;
            handlerContext.Response.TryGet(AuthorizationResponseParameters.Code, out authorizationCode);
            var refreshToken = await  _grantedTokenHelper.AddRefreshToken(handlerContext.Client.ClientId, authorizationCode, previousQueryParameters, handlerContext.Client.RefreshTokenExpirationTimeInSeconds, cancellationToken);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
        }
    }
}
