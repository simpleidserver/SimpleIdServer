// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.UI.ViewModels;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class ResetPasswordIndexViewModel : ISidStepViewModel
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string ConfirmedPassword { get; set; }
    public string ReturnUrl { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
    public string Realm { get; set; }
    public string CaptchaValue { get; set; }
    public string CaptchaType { get; set; }

    public List<string> Validate(ModelStateDictionary modelState)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(Login)) result.Add(Global.MissingLogin);
        if (string.IsNullOrWhiteSpace(Password)) result.Add(Global.MissingPassword);
        if (string.IsNullOrWhiteSpace(ConfirmedPassword)) result.Add(Global.MissingConfirmedPassword);
        if (Password != ConfirmedPassword) result.Add(Global.PasswordMismatch);
        return result;
    }
}
