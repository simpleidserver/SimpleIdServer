// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class ResetPasswordViewModel
{
    public string? NotificationMode { get; set; } = null;
    public string? Login { get; set; } = null;
    public string? Value { get; set; } = null;
    public bool IsResetLinkedSent { get; set; } = false;
    public string ReturnUrl { get; set; } = null!;

    public void Validate(ModelStateDictionary modelState)
    {
        if (string.IsNullOrWhiteSpace(Login))
            modelState.AddModelError("missing_login", "missing_login");
        if (string.IsNullOrWhiteSpace(Value))
            modelState.AddModelError("missing_value", "missing_value");
    }
}
