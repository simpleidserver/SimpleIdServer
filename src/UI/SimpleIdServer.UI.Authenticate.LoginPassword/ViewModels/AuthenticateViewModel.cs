// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SimpleIdServer.UI.Authenticate.LoginPassword.ViewModels
{
    public class AuthenticateViewModel
    {
        public AuthenticateViewModel() { }

        public AuthenticateViewModel(string login, string returnUrl)
        {
            Login = login;
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool RememberLogin { get; set; }

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
