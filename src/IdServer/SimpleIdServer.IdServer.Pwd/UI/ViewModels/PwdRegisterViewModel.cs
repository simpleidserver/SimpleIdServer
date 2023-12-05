// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class PwdRegisterViewModel : IRegisterViewModel
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string ConfirmedPassword { get; set; }
    public bool IsUpdated { get; set; }
    public bool IsNotAllowed { get; set; }
    public string Amr { get; set; }
    public List<string> Steps { get; set; }

    public void Validate(ModelStateDictionary modelState)
    {
        if (string.IsNullOrWhiteSpace(Login)) modelState.AddModelError("login_missing", "login_missing");
        if (string.IsNullOrWhiteSpace(Password)) modelState.AddModelError("password_missing", "password_missing");
        if (string.IsNullOrWhiteSpace(ConfirmedPassword)) modelState.AddModelError("confirmed_password_missing", "confirmed_password_missing");
        if (!string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(ConfirmedPassword) && Password != ConfirmedPassword) modelState.AddModelError("password_no_match", "password_no_match");
    }
}
