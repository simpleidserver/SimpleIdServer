﻿using MediatR;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenBankingApi.Accounts.Results;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Exceptions;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Accounts.Queries.Handlers
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, GetAccountsResult>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger<GetAccountQueryHandler> _logger;

        public GetAccountQueryHandler(
            IAccountRepository accountRepository,
            ITokenRepository tokenRepository,
            ILogger<GetAccountQueryHandler> logger)
        {
            _accountRepository = accountRepository;
            _tokenRepository = tokenRepository;
            _logger = logger;
        }

        public async Task<GetAccountsResult> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var token = await _tokenRepository.Get(request.Token, cancellationToken);
            if (token == null)
            {
                _logger.LogError($"Access token '{request.Token}' is invalid or has been revoked");
                throw new UnauthorizedException(string.Format(Global.AccessTokenInvalid, token));
            }

            var account = await _accountRepository.Get(request.AccountId, cancellationToken);
            if (account == null)
            {
                _logger.LogError($"the account '{request.AccountId}' doesn't exist");
                throw new UnknownAccountException(string.Format(string.Format(Global.UnknownAccount, request.AccountId)));
            }

            var url = $"{request.Issuer}/{Constants.RouteNames.AccountAccessContents}/{request.AccountId}";
            return GetAccountsResult.ToResult(new[] { account }, new List<AccountAccessConsentPermission>(), url, 1);
        }
    }
}
