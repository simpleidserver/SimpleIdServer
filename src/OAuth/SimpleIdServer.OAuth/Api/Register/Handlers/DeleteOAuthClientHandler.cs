// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OAuth.Persistence.Parameters;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register.Handlers
{
    public interface IDeleteOAuthClientHandler
    {
        Task Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken);
    }

    public class DeleteOAuthClientHandler : BaseOAuthClientHandler, IDeleteOAuthClientHandler
    {
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly ITokenRepository _tokenRepository;

        public DeleteOAuthClientHandler(IGrantedTokenHelper grantedTokenHelper,
            ITokenRepository tokenQueryRepository,
            IOAuthClientRepository oAuthClientRepository,
            ILogger<BaseOAuthClientHandler> logger) : base(oAuthClientRepository, logger)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _tokenRepository = tokenQueryRepository;
        }

        public virtual async Task Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var oauthClient = await GetClient(clientId, handlerContext, cancellationToken);
            var searchResult = await _tokenRepository.Find(new SearchTokenParameter
            {
                ClientId = clientId
            }, cancellationToken);
            if (searchResult.Content.Any())
            {
                await _grantedTokenHelper.RemoveTokens(searchResult.Content, cancellationToken);
                Logger.LogInformation($"the tokens '{string.Join(",", searchResult.Content.Select(_ => _.Id))}' have been revoked");
            }

            Logger.LogInformation($"the client '{clientId}' has been removed");
            await OAuthClientRepository.Delete(oauthClient, cancellationToken);
        }
    }
}
