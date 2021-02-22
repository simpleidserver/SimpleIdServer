// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MediatR;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.AccountAccessContents.Commands
{
    public class ConfirmAccountAccessConsentCommand : IRequest<bool>
    {
        public ConfirmAccountAccessConsentCommand(string consentId, IEnumerable<string> accountIds)
        {
            ConsentId = consentId;
            AccountIds = accountIds;
        }

        public string ConsentId { get; set; }
        public IEnumerable<string> AccountIds { get; set; }
    }
}