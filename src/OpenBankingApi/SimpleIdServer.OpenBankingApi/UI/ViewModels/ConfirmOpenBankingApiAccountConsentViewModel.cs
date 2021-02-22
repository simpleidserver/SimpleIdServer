// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.UI.ViewModels
{
    public class ConfirmOpenBankingApiAccountConsentViewModel
    {
        public string ConsentId { get; set; }
        public IEnumerable<string> SelectedAccounts { get; set; }
        public string ReturnUrl { get; set; }
    }
}
