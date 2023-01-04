// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.TokenBuilders
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
        Task Build(IEnumerable<string> scopes, Dictionary<string, object> claims, HandlerContext handlerContext, CancellationToken cancellationToken);
        /// <summary>
        /// Refresh token from previous one.
        /// </summary>
        /// <param name="previousQueryParameters"></param>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        Task Refresh(JsonObject previousQueryParameters, HandlerContext handlerContext, CancellationToken token);
    }
}