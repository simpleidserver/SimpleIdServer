// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.UIs;
using SimpleIdServer.IdServer.Resources;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public abstract class OTPRegisterViewModel : IRegisterViewModel
{
    public string NameIdentifier {  get; set; }
    public string Value { get; set; }
    public bool IsVerified { get; set; }
    public string OTPCode { get; set; } = null!;
    public string Action { get; set; } = null!;
    public bool IsUpdated { get; set; } = false;
    public bool IsOTPCodeSent { get; set; } = false;
    public bool IsNotAllowed { get; set; }
    public string Amr { get; set; }
    public List<string> Steps { get; set; }
    public string? RedirectUrl { get; set; }
    public bool IsCreated { get; set; }
    public string StepId { get; set; }
    public string WorkflowId { get; set; }
    public string CurrentLink { get; set; }
    public string Realm { get; set; }

    public List<string> Validate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(Value)) result.Add(Global.MissingValue);
        if (string.IsNullOrWhiteSpace(Action)) result.Add(Global.MissingAction);
        else if (Action != "SENDCONFIRMATIONCODE" && Action != "REGISTER") result.Add(Global.ActionIsInvalid);
        if (Action == "REGISTER")
        {
            if (string.IsNullOrWhiteSpace(OTPCode)) result.Add(Global.MissingOtpCode);
            else if (!long.TryParse(OTPCode, out long l)) result.Add(Global.OtpCodeMustBeNumber);
            else result.AddRange(SpecificValidate());
        }
        else result.AddRange(SpecificValidate());
        return result;
    }

    public abstract List<string> SpecificValidate();
}
