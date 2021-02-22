using MediatR;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Exceptions;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenBankingApi.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands.Handlers
{
    public class RejectAccountAccessConsentCommandHandler : IRequestHandler<RejectAccountAccessConsentCommand, bool>
    {
        private readonly ICommandRepository _commandRepository;
        private readonly ILogger<RejectAccountAccessConsentCommandHandler> _logger;

        public RejectAccountAccessConsentCommandHandler(
            ICommandRepository commandRepository,
            ILogger<RejectAccountAccessConsentCommandHandler> logger)
        {
            _commandRepository = commandRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectAccountAccessConsentCommand request, CancellationToken cancellationToken)
        {
            var accountAccessConsent = _commandRepository.GetLastAggregate<AccountAccessConsentAggregate>(request.ConsentId);
            if (accountAccessConsent == null)
            {
                _logger.LogError($"Access Access Consent '{request.ConsentId}' doesn't exist");
                throw new UnknownAccountAccessConsentException(string.Format(Global.UnknownAccountAccessConsent, request.ConsentId));
            }

            accountAccessConsent.Reject();
            await _commandRepository.Commit(accountAccessConsent, cancellationToken);
            return true;
        }
    }
}
