// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public abstract class OTPRegisterViewModel : IRegisterViewModel, IOTPViewModel
{
    public string Value { get; set; }
    public bool IsVerified { get; set; }
    public string OTPCode { get; set; } = null!;
    public string Action { get; set; } = null!;
    public bool IsUpdated { get; set; } = false;
    public string? ReturnUrl { get; set; }
    public bool IsCreated { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
    public string Realm { get; set; }
    public bool UpdateOneCredential { get; set; }
    public string CaptchaValue { get; set; }
    public string CaptchaType { get; set; }

    public List<string> Validate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(Value)) result.Add(RegisterFormErrorMessages.MissingValue);
        if (string.IsNullOrWhiteSpace(Action)) result.Add(RegisterFormErrorMessages.MissingAction);
        else if (Action != "SENDCONFIRMATIONCODE" && Action != "REGISTER") result.Add(RegisterFormErrorMessages.ActionIsInvalid);
        if (Action == "REGISTER")
        {
            if (string.IsNullOrWhiteSpace(OTPCode)) result.Add(RegisterFormErrorMessages.MissingOtpCode);
            else if (!long.TryParse(OTPCode, out long l)) result.Add(RegisterFormErrorMessages.OtpCodeMustBeNumber);
            else result.AddRange(SpecificValidate());
        }
        else result.AddRange(SpecificValidate());
        return result;
    }

    public abstract List<string> SpecificValidate();
}
