using MediatR;
using SimpleIdServer.OpenBankingApi.AccountAccessContents.Results;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Persistences;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands.Handlers
{
    public class AddAccountAccessContentCommandHandler : IRequestHandler<AddAccountAccessContentCommand, AccountAccessContentResult>
    {
        private readonly ICommandRepository _commandRepository;

        public AddAccountAccessContentCommandHandler(ICommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }

        public async Task<AccountAccessContentResult> Handle(AddAccountAccessContentCommand request, CancellationToken cancellationToken)
        {
            var risk = request.Risk == null ? string.Empty : request.Risk.ToString();
            var accountAccessConsent = AccountAccessConsentAggregate.Create(request.Data.Permissions, request.Data.ExpirationDateTime, request.Data.TransactionFromDateTime, request.Data.TransactionToDateTime, risk);
            await _commandRepository.Commit(accountAccessConsent, cancellationToken);
            var url = $"{request.Issuer}/{Constants.RouteNames.AccountAccessContents}/{accountAccessConsent.AggregateId}";
            return AccountAccessContentResult.ToDto(accountAccessConsent, url, 1);
        }
    }
}