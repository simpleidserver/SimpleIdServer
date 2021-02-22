// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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
    public class ConfirmAccountAccessConsentCommandHandler : IRequestHandler<ConfirmAccountAccessConsentCommand, bool>
    {
        private readonly ICommandRepository _commandRepository;
        private readonly ILogger<ConfirmAccountAccessConsentCommandHandler> _logger;

        public ConfirmAccountAccessConsentCommandHandler(
            ICommandRepository commandRepository,
            ILogger<ConfirmAccountAccessConsentCommandHandler> logger)
        {
            _commandRepository = commandRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ConfirmAccountAccessConsentCommand request, CancellationToken cancellationToken)
        {
            var accountAccessConsent = _commandRepository.GetLastAggregate<AccountAccessConsentAggregate>(request.ConsentId);
            if (accountAccessConsent == null)
            {
                _logger.LogError($"Access Access Consent '{request.ConsentId}' doesn't exist");
                throw new UnknownAccountAccessConsentException(string.Format(Global.UnknownAccountAccessConsent, request.ConsentId));
            }
            
            accountAccessConsent.Confirm(request.AccountIds);
            await _commandRepository.Commit(accountAccessConsent, cancellationToken);
            return true;
        }
    }
}
