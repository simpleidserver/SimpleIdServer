// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OpenBankingApi.Domains.AccountAccessConsent.Enums;
using SimpleIdServer.OpenBankingApi.Persistences;
using SimpleIdServer.OpenID.Api.BCAuthorize;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenBankingApi.Api.BCAuthorize
{
    public class OpenBankingBCAuthorizeHandler : BCAuthorizeHandler
    {
        private readonly IAccountAccessConsentRepository _accountAccessConsentRepository;
        private readonly IAccountRepository _accountRepository;

        public OpenBankingBCAuthorizeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IBCAuthorizeRequestValidator bcAuthorizeRequestValidator,
            IBCNotificationService bcNotificationService,
            IBCAuthorizeRepository bcAuthorizeRepository,
            IAccountAccessConsentRepository accountAccessConsentRepository,
            IAccountRepository accountRepository,
            IOptions<OpenIDHostOptions> options) : base(clientAuthenticationHelper, bcAuthorizeRequestValidator, bcNotificationService, bcAuthorizeRepository, options)
        {
            _accountAccessConsentRepository = accountAccessConsentRepository;
            _accountRepository = accountRepository;
        }

        protected override async Task<IEnumerable<BCAuthorizePermission>> GetPermissions(string clientId, string subject, CancellationToken cancellationToken)
        {
            var consents = await _accountAccessConsentRepository.GetAwaitingAuthorisationAccountAccessConsents(clientId, cancellationToken);
            var result = new List<BCAuthorizePermission>();
            foreach(var consent in consents)
            {
                if (consent.Permissions.Contains(AccountAccessConsentPermission.ReadAccountsBasic) || consent.Permissions.Contains(AccountAccessConsentPermission.ReadAccountsDetail))
                {
                    var accounts = await _accountRepository.GetBySubject(subject, cancellationToken);
                    foreach(var account in accounts)
                    {
                        result.Add(BCAuthorizePermission.Create(
                            consent.AggregateId,
                            OpenBankingApiConstants.NotifiableConsentTypes.AccountAccessConsent,
                            account.AggregateId,
                            account.AccountSubType?.Name
                        ));
                    }
                }
            }

            return result;
        }

        protected override async Task ConfirmPermissions(IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            foreach(var grp in permissions.GroupBy(p => p.ConsentId))
            {
                var accountAccessConsent = await _accountAccessConsentRepository.Get(grp.Key, cancellationToken);
                accountAccessConsent.Confirm(grp.Select(p => p.PermissionId));
                await _accountAccessConsentRepository.Update(accountAccessConsent, cancellationToken);
            }

            await _accountAccessConsentRepository.SaveChanges(cancellationToken);
        }

        protected override async Task RejectPermissions(IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            foreach(var permission in permissions)
            {
                var accountAccessConsent = await _accountAccessConsentRepository.Get(permission.ConsentId, cancellationToken);
                accountAccessConsent.Reject();
                await _accountAccessConsentRepository.Update(accountAccessConsent, cancellationToken);
            }

            await _accountAccessConsentRepository.SaveChanges(cancellationToken);
        }
    }
}
