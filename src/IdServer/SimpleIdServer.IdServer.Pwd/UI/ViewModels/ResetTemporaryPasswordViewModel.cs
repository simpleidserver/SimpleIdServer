// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class ResetTemporaryPasswordViewModel : AuthenticatePasswordViewModel
{
    public string? ConfirmationPassword { get; set; } = null;

    public override List<string> Validate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(Login)) result.Add(Global.MissingConfirmationCode);
        if (string.IsNullOrWhiteSpace(Password)) result.Add(Global.MissingPassword);
        if (string.IsNullOrWhiteSpace(ConfirmationPassword)) result.Add(Global.MissingConfirmedPassword);
        if (Password != ConfirmationPassword) result.Add(Global.PasswordMismatch);
        return result;
    }
}
