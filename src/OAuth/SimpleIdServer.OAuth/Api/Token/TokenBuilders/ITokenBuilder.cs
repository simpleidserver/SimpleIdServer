// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
        /// <param name="claims"></param>
        /// <returns></returns>
        Task Build(IEnumerable<string> scopes, HandlerContext handlerContext, JObject claims = null);
        /// <summary>
        /// Refresh token from previous one.
        /// </summary>
        /// <param name="previousQueryParameters"></param>
        /// <param name="handlerContext"></param>
        /// <returns></returns>
        Task Refresh(JObject previousQueryParameters, HandlerContext handlerContext);
    }
}