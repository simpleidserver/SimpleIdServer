// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Resources;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels
{
    public class AuthenticatePasswordViewModel : BaseAuthenticateViewModel
    {
        public AuthenticatePasswordViewModel() { }

        public string Password { get; set; }

        public override List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(ReturnUrl))
                errors.Add(AuthFormErrorMessages.MissingReturnUrl);

            if (string.IsNullOrWhiteSpace(Login))
                errors.Add(AuthFormErrorMessages.MissingLogin);

            if (string.IsNullOrWhiteSpace(Password))
                errors.Add(AuthFormErrorMessages.MissingPassword);

            return errors;
        }
    }
}