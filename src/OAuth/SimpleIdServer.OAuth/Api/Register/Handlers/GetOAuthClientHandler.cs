// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.Store;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IGetOAuthClientHandler
    {
        Task<JsonObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken);
    }

    public class GetOAuthClientHandler : BaseOAuthClientHandler, IGetOAuthClientHandler
    {
        public GetOAuthClientHandler(IClientRepository oauthClientRepository, ILogger<BaseOAuthClientHandler> logger) : base(oauthClientRepository, logger)
        {
        }

        public virtual async Task<JsonObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var client = await GetClient(clientId, handlerContext, cancellationToken);
            return BuildResponse(client, handlerContext.Request.IssuerName);
        }

        protected virtual JsonObject BuildResponse(Client oauthClient, string issuer)
        {
            return oauthClient.ToDto(issuer);
        }
    }
}
