// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class PwdRegisterViewModel : IRegisterViewModel
{
    public string? Login { get; set; }
    public string Password { get; set; }
    public string ConfirmedPassword { get; set; }
    public bool IsUpdated { get; set; }
    public string? ReturnUrl { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
    public bool IsCreated { get; set; }
    public string Realm { get; set; }
    public bool UpdateOneCredential { get; set; }

    public List<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Login)) errors.Add(RegisterFormErrorMessages.MissingLogin);
        if (string.IsNullOrWhiteSpace(Password)) errors.Add(RegisterFormErrorMessages.MissingPassword);
        if (string.IsNullOrWhiteSpace(ConfirmedPassword)) errors.Add(RegisterFormErrorMessages.MissingConfirmedPassword);
        if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmedPassword) && Password != ConfirmedPassword) errors.Add(RegisterFormErrorMessages.PasswordMismatch);
        return errors;
    }
}
