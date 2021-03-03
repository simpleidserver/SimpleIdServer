// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token.TokenBuilders
{
    public interface ITokenBuilder
    {
        string Name { get; }
        /// <summary>
        /// Build new token.
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, CancellationToken cancellationToken);
        /// <summary>
        /// Build new token.
        /// </summary>
        /// <param name="scopes"></param>
        /// <param name="claims"></param>
        /// <param name="handlerContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Build(IEnumerable<string> scopes, JObject claims, HandlerContext handlerContext, CancellationToken cancellationToken);
        /// <summary>
        /// Refresh token from previous one.
        /// </summary>
        /// <param name="previousQueryParameters"></param>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        Task Refresh(JObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token);
    }
}