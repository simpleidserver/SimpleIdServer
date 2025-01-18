// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Email.Resources;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI.ViewModels
{
    public class AuthenticateEmailViewModel : BaseOTPAuthenticateViewModel
    {
        public AuthenticateEmailViewModel() { }

        public override List<string> SpecificValidate()
        {
            var errors = new List<string>();
            if (!string.IsNullOrWhiteSpace(Login) && !EmailValidator.IsValidEmail(Login))
                errors.Add(Global.InvalidEmail);
            return errors;
        }
    }
}
