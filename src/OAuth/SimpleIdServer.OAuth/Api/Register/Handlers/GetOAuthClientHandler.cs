// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OAuth.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IGetOAuthClientHandler
    {
        Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken);
    }

    public class GetOAuthClientHandler : BaseOAuthClientHandler, IGetOAuthClientHandler
    {
        public GetOAuthClientHandler(IOAuthClientRepository oauthClientRepository, ILogger<BaseOAuthClientHandler> logger) : base(oauthClientRepository, logger)
        {
        }

        public virtual async Task<JObject> Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var client = await GetClient(clientId, handlerContext, cancellationToken);
            return BuildResponse(client, handlerContext.Request.IssuerName);
        }

        protected virtual JObject BuildResponse(BaseClient oauthClient, string issuer)
        {
            return oauthClient.ToDto(issuer);
        }
    }
}
