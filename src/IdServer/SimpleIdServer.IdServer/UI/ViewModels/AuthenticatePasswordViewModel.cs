// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class AuthenticatePasswordViewModel : BaseAuthenticateViewModel
    {
        public AuthenticatePasswordViewModel() { }

        public string Password { get; set; }
        public bool CanResetPassword { get; set; }

        public override void CheckRequiredFields(ModelStateDictionary modelStateDictionary)
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