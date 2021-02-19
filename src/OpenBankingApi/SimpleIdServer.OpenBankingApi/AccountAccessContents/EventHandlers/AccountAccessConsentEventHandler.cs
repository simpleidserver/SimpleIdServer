using SimpleIdServer.OpenBankingApi.Domains;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Events;
using SimpleIdServer.OpenBankingApi.Persistences;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.EventHandlers
{
    public class AccountAccessConsentEventHandler : IEventHandler<AccountAccessConsentAddedEvent>
    {
        private readonly IAccountAccessConsentCommandRepository _accountAccessConsentCommandRepository;

        public AccountAccessConsentEventHandler(
            IAccountAccessConsentCommandRepository accountAccessConsentCommandRepository)
        {
            _accountAccessConsentCommandRepository = accountAccessConsentCommandRepository;
        }

        public async Task Handle(AccountAccessConsentAddedEvent evt, CancellationToken cancellationToken)
        {
            var result = AccountAccessConsentAggregate.Build(new List<DomainEvent>
            {
                evt
            });
            await _accountAccessConsentCommandRepository.Add(result, cancellationToken);
            await _accountAccessConsentCommandRepository.SaveChanges(cancellationToken);
        }
    }
}
