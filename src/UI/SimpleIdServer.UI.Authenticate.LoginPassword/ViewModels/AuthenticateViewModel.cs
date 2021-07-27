// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.OpenID.UI.ViewModels;
using System.Collections.Generic;

namespace SimpleIdServer.UI.Authenticate.LoginPassword.ViewModels
{
    public class AuthenticateViewModel
    {
        public AuthenticateViewModel() 
        {
            ExternalIdsProviders = new List<ExternalIdProvider>();
        }

        public AuthenticateViewModel(string login, string returnUrl, string clientName, string logoUri, string tosUri, string policyUri, ICollection<ExternalIdProvider> externalIdProviders) : this()
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
        public string ReturnUrl { get; set; }
        public string ClientName { get; set; }
        public string LogoUri { get; set; }
        public string TosUri { get; set; }
        public string PolicyUri { get; set; }
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public ICollection<ExternalIdProvider> ExternalIdsProviders { get; set; }

        public void Check(ModelStateDictionary modelStateDictionary)
        {
            if (string.IsNullOrWhiteSpace(ReturnUrl))
            {
                modelStateDictionary.AddModelError("missing_return_url", "missing_return_url");
            }

            if (string.IsNullOrWhiteSpace(Login))
            {
                modelStateDictionary.AddModelError("missing_login", "missing_login");
            }
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                modelStateDictionary.AddModelError("missing_password", "missing_password");
            }
        }
    }
}
