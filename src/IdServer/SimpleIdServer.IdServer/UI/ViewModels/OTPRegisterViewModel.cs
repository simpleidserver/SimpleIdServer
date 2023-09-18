// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class OTPRegisterViewModel
{
    public string NameIdentifier {  get; set; }
    public string Value { get; set; }
    public bool IsVerified { get; set; }
    public string OTPCode { get; set; } = null!;
    public string Action { get; set; } = null!;
    public bool IsRegistered { get; set; } = false;
    public bool IsUpdated { get; set; } = false;
    public bool IsOTPCodeSent { get; set; } = false;

    public void Validate(ModelStateDictionary modelState)
    {
        if (string.IsNullOrWhiteSpace(Value)) modelState.AddModelError("value_missing", "value_missing");
        if (string.IsNullOrWhiteSpace(OTPCode)) modelState.AddModelError("otpcode_missing", "otpcode_missing");
        else if (long.TryParse(OTPCode, out long l)) modelState.AddModelError("otpcode_not_number", "otpcode_not_number");
        if (string.IsNullOrWhiteSpace(Action)) modelState.AddModelError("action_missing", "action_missing");
        else if (Action != "SENDCONFIRMATIONCODE" && Action != "REGISTER") modelState.AddModelError("invalid_action", "invalid_action");
    }
}
