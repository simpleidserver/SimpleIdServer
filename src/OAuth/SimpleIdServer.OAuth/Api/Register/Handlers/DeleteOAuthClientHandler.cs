// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Helpers;
using SimpleIdServer.Store;
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
            IClientRepository clientRepository,
            ILogger<BaseOAuthClientHandler> logger) : base(clientRepository, logger)
        {
            _grantedTokenHelper = grantedTokenHelper;
            _tokenRepository = tokenQueryRepository;
        }

        public virtual async Task Handle(string clientId, HandlerContext handlerContext, CancellationToken cancellationToken)
        {
            var oauthClient = await GetClient(clientId, handlerContext, cancellationToken);
            var searchResult = await _tokenRepository.Query().Where(t => t.ClientId == clientId).ToListAsync(cancellationToken);
            if (searchResult.Any())
            {
                await _grantedTokenHelper.RemoveTokens(searchResult, cancellationToken);
                Logger.LogInformation($"the tokens '{string.Join(",", searchResult.Select(_ => _.Id))}' have been revoked");
            }

            Logger.LogInformation($"the client '{clientId}' has been removed");
            ClientRepository.Delete(oauthClient);
            await ClientRepository.SaveChanges(cancellationToken);
        }
    }
}
