// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class AccountsIndexViewModel : ILayoutViewModel
{
    public AccountsIndexViewModel(string returnUrl, IEnumerable<AccountViewModel> accounts, List<Language> languages)
    {
        ReturnUrl = returnUrl;
        Accounts = accounts;
        Languages = languages;
    }

    public string ReturnUrl { get; }
    public IEnumerable<AccountViewModel> Accounts { get; set; }
    public List<Language> Languages { get; set; }
}