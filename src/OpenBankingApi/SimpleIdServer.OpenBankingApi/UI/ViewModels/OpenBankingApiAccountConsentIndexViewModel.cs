// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.UI.ViewModels
{
    public class OpenBankingApiAccountConsentIndexViewModel
    {
        public OpenBankingApiAccountConsentIndexViewModel(
            string returnUrl, 
            string clientName, 
            ICollection<OpenBankingApiAccountViewModel> accounts,
            OpenBankingApiConsentAccountViewModel consent)
        {
            ReturnUrl = returnUrl;
            ClientName = clientName;
            Accounts = accounts;
            Consent = consent;
        }

        public string ReturnUrl { get; set; }
        public string ClientName { get; set; }
        public ICollection<OpenBankingApiAccountViewModel> Accounts { get; set; }
        public OpenBankingApiConsentAccountViewModel Consent { get; set; }
    }
}
