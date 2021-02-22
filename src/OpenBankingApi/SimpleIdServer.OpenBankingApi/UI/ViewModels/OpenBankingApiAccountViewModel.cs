// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.UI.ViewModels
{
    public class OpenBankingApiAccountViewModel
    {
        public OpenBankingApiAccountViewModel()
        {
            CashAccounts = new List<OpenBankingApiCashAccountViewModel>();
        }

        public string Id { get; set; }
        public string AccountSubType { get; set; }
        public IEnumerable<OpenBankingApiCashAccountViewModel> CashAccounts { get; set; }
    }
}
