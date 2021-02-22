using SimpleIdServer.OpenBankingApi.Domains;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Events;
using SimpleIdServer.OpenBankingApi.Persistences;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.EventHandlers
{
    public class AccountAccessConsentEventHandler : 
        IEventHandler<AccountAccessConsentAddedEvent>,
        IEventHandler<AccountAccessConsentConfirmedEvent>
    {
        private readonly IAccountAccessConsentRepository _accountAccessConsentRepository;

        public AccountAccessConsentEventHandler(
            IAccountAccessConsentRepository accountAccessConsentRepository)
        {
            _accountAccessConsentRepository = accountAccessConsentRepository;
        }

        public async Task Handle(AccountAccessConsentAddedEvent evt, CancellationToken cancellationToken)
        {
            var result = AccountAccessConsentAggregate.Build(new List<DomainEvent>
            {
                evt
            });
            await _accountAccessConsentRepository.Add(result, cancellationToken);
            await _accountAccessConsentRepository.SaveChanges(cancellationToken);
        }

        public async Task Handle(AccountAccessConsentConfirmedEvent evt, CancellationToken cancellationToken)
        {
            var result = await _accountAccessConsentRepository.Get(evt.AggregateId, cancellationToken);
            result.Handle(evt);
            await _accountAccessConsentRepository.Update(result, cancellationToken);
            await _accountAccessConsentRepository.SaveChanges(cancellationToken);
        }
    }
}
