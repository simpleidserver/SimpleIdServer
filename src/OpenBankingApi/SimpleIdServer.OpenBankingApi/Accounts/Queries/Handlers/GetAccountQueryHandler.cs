using MediatR;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OpenBankingApi.Accounts.Results;
using SimpleIdServer.OpenBankingApi.Exceptions;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Accounts.Queries.Handlers
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountResult>
    {
        private readonly IAccountQueryRepository _accountQueryRepository;
        private readonly ILogger<GetAccountQueryHandler> _logger;

        public GetAccountQueryHandler(
            IAccountQueryRepository accountQueryRepository,
            ILogger<GetAccountQueryHandler> logger)
        {
            _accountQueryRepository = accountQueryRepository;
            _logger = logger;
        }

        public async Task<AccountResult> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountQueryRepository.Get(request.AccountId);
            if (account == null)
            {
                _logger.LogError($"the account '{request.AccountId}' doesn't exist");
                throw new UnknownAccountException(string.Format(string.Format(Global.UnknownAccount, request.AccountId)));
            }

            return AccountResult.ToResult(account);
        }
    }
}
