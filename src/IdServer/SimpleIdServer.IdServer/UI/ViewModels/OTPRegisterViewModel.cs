// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.UIs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public abstract class OTPRegisterViewModel : StepViewModel, IRegisterViewModel
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

    public void Validate(ModelStateDictionary modelState)
    {
        if (string.IsNullOrWhiteSpace(Value)) modelState.AddModelError("value_missing", "value_missing");
        if (string.IsNullOrWhiteSpace(Action)) modelState.AddModelError("action_missing", "action_missing");
        else if (Action != "SENDCONFIRMATIONCODE" && Action != "REGISTER") modelState.AddModelError("invalid_action", "invalid_action");
        if (Action == "REGISTER")
        {
            if (string.IsNullOrWhiteSpace(OTPCode)) modelState.AddModelError("otpcode_missing", "otpcode_missing");
            else if (!long.TryParse(OTPCode, out long l)) modelState.AddModelError("otpcode_not_number", "otpcode_not_number");
            else SpecificValidate(modelState);
        }
        else SpecificValidate(modelState);
    }

    public abstract void SpecificValidate(ModelStateDictionary modelState);
}
