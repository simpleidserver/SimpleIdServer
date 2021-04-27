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
        private readonly ITokenQueryRepository _tokenQueryRepository;
        private readonly IOAuthUserCommandRepository _oauthUserCommandRepository;

        public DeleteOAuthClientHandler(IGrantedTokenHelper grantedTokenHelper,
            ITokenQueryRepository tokenQueryRepository,
            IOAuthUserCommandRepository oAuthUserCommandRepository,
            IOAuthClientQueryRepository oauthClientQueryRepository, 
            IOAuthClientCommandRepository oAuthClientCommandRepository,
            ILogger<BaseOAuthClientHandler> logger) : base(oauthClientQueryRepository, oAuthClientCommandRepository, logger)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _tokenQueryRepository = tokenQueryRepository;
            _oauthUserCommandRepository = oAuthUserCommandRepository;
        }

        public virtual async Task Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var oauthClient = await GetClient(clientId, handlerContext, cancellationToken);
            var searchResult = await _tokenQueryRepository.Find(new SearchTokenParameter
            {
                ClientId = clientId
            }, cancellationToken);
            if (searchResult.Content.Any())
            {
                await _grantedTokenHelper.RemoveTokens(searchResult.Content, cancellationToken);
                Logger.LogInformation($"the tokens '{string.Join(",", searchResult.Content.Select(_ => _.Id))}' have been revoked");
            }

            await _oauthUserCommandRepository.RemoveAllConsents(clientId, cancellationToken);
            Logger.LogInformation($"the client '{clientId}' has been removed");
            await OAuthClientCommandRepository.Delete(oauthClient, cancellationToken);
        }
    }
}
