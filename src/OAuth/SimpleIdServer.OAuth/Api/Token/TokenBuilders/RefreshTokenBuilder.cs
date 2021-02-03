// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
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

        public virtual Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, JObject claims = null)
        {
            var dic = new JObject();
            if (handlerContext.Request.Data != null)
            {
                foreach (var record in handlerContext.Request.Data)
                {
                    dic.Add(record.Key, record.Value);
                }
            }

            var refreshToken = _grantedTokenHelper.BuildRefreshToken(dic, handlerContext.Client.RefreshTokenExpirationTimeInSeconds);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
            return Task.FromResult(0);
        }

        public virtual Task Refresh(JObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token)
        {
            var refreshToken = _grantedTokenHelper.BuildRefreshToken(previousQueryParameters, handlerContext.Client.RefreshTokenExpirationTimeInSeconds);
            handlerContext.Response.Add(TokenResponseParameters.RefreshToken, refreshToken);
            return Task.FromResult(0);
        }
    }
}
