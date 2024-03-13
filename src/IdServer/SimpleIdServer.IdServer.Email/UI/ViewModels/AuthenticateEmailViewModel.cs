// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI.ViewModels
{
    public class AuthenticateEmailViewModel : BaseOTPAuthenticateViewModel
    {
        public AuthenticateEmailViewModel() { }

        public override void SpecificValidate(ModelStateDictionary modelStateDictionary)
        {
            if (!string.IsNullOrWhiteSpace(Login) && !EmailValidator.IsValidEmail(Login)) modelStateDictionary.AddModelError("invalid_email", "invalid_email");
        }
    }
}
