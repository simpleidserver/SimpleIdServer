// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class AccountsIndexViewModel
    {
        public AccountsIndexViewModel(string returnUrl, IEnumerable<AccountViewModel> accounts)
        {
            ReturnUrl = returnUrl;
            Accounts = accounts;
        }

        public string ReturnUrl { get; }
        public IEnumerable<AccountViewModel> Accounts { get; }
    }
}
