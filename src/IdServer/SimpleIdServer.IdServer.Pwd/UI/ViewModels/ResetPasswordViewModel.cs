// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.UIs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Pwd.UI.ViewModels;

public class ResetPasswordViewModel : IStepViewModel
{
    public string? NotificationMode { get; set; } = null;
    public string? Login { get; set; } = null;
    public string? Value { get; set; } = null;
    public bool IsResetLinkedSent { get; set; } = false;
    public string ReturnUrl { get; set; } = null!;
    public string Realm { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }

    public List<string> Validate(ModelStateDictionary modelState)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(Login))
            result.Add(Global.MissingLogin);
        if (string.IsNullOrWhiteSpace(Value))
            result.Add(Global.MissingValue);
        return result;
    }
}
