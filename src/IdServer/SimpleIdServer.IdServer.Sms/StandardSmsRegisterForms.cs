// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Sms;

public static class StandardSmsRegisterForms
{
    public static string smsSendConfirmationCodeFormId = "bf574d6c-dc08-4cb1-a16a-24e596bd4e13";
    public static string smsRegisterFormId = "520cfd8c-4f98-4228-bf6b-b8ff7ba92272";
    public static string backButtonId = "93aeef90-acd7-4c90-9c13-06c891eec3ee";

    public static FormRecord SmsForm = OtpRegisterLayoutBuilder.New("d877be50-84a4-4af2-9c96-c4775fac7e65", "smsRegister", Constants.AMR, LayoutTranslations.PhoneNumber)
        .ConfigureSendConfirmationCode(smsSendConfirmationCodeFormId)
        .ConfigureRegistration(smsRegisterFormId)
        .ConfigureBackButton(backButtonId)
        .AddErrorMessage(RegisterFormErrorMessages.NotAllowedToRegister, Global.NotAllowedToRegister)
        .AddErrorMessage(RegisterFormErrorMessages.MissingValue, Global.MissingValue)
        .AddErrorMessage(RegisterFormErrorMessages.MissingAction, Global.MissingAction)
        .AddErrorMessage(RegisterFormErrorMessages.ActionIsInvalid, Global.ActionIsInvalid)
        .AddErrorMessage(RegisterFormErrorMessages.MissingOtpCode, Global.MissingOtpCode)
        .AddErrorMessage(RegisterFormErrorMessages.OtpCodeMustBeNumber, Global.OtpCodeMustBeNumber)
        .AddErrorMessage(RegisterFormErrorMessages.UserWithSameClaimAlreadyExists, Global.UserWithSameClaimAlreadyExists)
        .AddErrorMessage(RegisterFormErrorMessages.OtpCodeIsInvalid, Global.OtpCodeIsInvalid)
        .AddErrorMessage(RegisterFormErrorMessages.ImpossibleToSendOtpCode, Global.ImpossibleToSendOtpCode)
        .AddSuccessMessage(RegisterFormSuccessMessages.OtpCodeIsSent, Global.OtpCodeIsSent)
        .Build(DateTime.UtcNow);
}
