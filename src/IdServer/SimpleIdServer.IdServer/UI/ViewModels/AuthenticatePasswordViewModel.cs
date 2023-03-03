// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class AuthenticatePasswordViewModel : BaseAuthenticateViewModel
    {
        public AuthenticatePasswordViewModel() { }

        public AuthenticatePasswordViewModel(string returnUrl, ICollection<ExternalIdProvider> externalIdProviders)
        {
            ReturnUrl = returnUrl;
            ExternalIdsProviders = externalIdProviders;
        }

        public AuthenticatePasswordViewModel(string login, string returnUrl, string clientName, string logoUri, string tosUri, string policyUri, ICollection<ExternalIdProvider> externalIdProviders)
        {
            Login = login;
            ReturnUrl = returnUrl;
            ClientName = clientName;
            LogoUri = logoUri;
            TosUri = tosUri;
            PolicyUri = policyUri;
            ExternalIdsProviders = externalIdProviders;
        }

        public string Login { get; set; }
        public string Password { get; set; }

        public void Check(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");

            if (string.IsNullOrWhiteSpace(Login))
                modelStateDictionary.AddModelError("missing_login", "missing_login");

            if (string.IsNullOrWhiteSpace(Password))
                modelStateDictionary.AddModelError("missing_password", "missing_password");
        }
    }
}
