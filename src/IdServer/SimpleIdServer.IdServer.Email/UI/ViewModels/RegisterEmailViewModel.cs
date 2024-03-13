// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Email.UI.ViewModels;

public class RegisterEmailViewModel : OTPRegisterViewModel
{
    public override void SpecificValidate(ModelStateDictionary modelState)
    {
        if (!string.IsNullOrWhiteSpace(Value) && !EmailValidator.IsValidEmail(Value)) modelState.AddModelError("invalid_email", "invalid_email");
    }
}
