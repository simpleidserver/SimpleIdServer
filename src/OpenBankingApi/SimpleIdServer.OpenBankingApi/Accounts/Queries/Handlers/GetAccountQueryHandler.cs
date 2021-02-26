using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IAccountRepository _accountQueryRepository;
        private readonly ILogger<GetAccountQueryHandler> _logger;

        public GetAccountQueryHandler(
            IAccountRepository accountQueryRepository,
            ILogger<GetAccountQueryHandler> logger)
        {
            _accountQueryRepository = accountQueryRepository;
            _logger = logger;
        }

        public async Task<GetAccountsResult> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountQueryRepository.Get(request.AccountId, cancellationToken);
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
