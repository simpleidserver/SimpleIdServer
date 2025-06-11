// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Sms;

public static class StandardSmsAuthForms
{
    public static string smsAuthForm = "c7a16cfb-33bf-40c0-a0ce-827ff25e441b";
    public static string smsSendConfirmationCode = "b08e3449-b912-4bf7-9b9d-76a8054e5ff2";


    public static FormRecord SmsForm = OtpAuthFormLayoutBuilder.New("3781726f-07a0-48ad-8619-91323102d489", "smsAuth", Constants.AMR, LayoutTranslations.PhoneNumber)
        .ConfigureSendConfirmationCode(smsSendConfirmationCode)
        .ConfigureAuthentication(smsAuthForm)
        .AddErrorMessage(AuthFormErrorMessages.MaximumNumberActiveSessions, Global.MaximumNumberActiveSessions)
        .AddErrorMessage(AuthFormErrorMessages.MissingLogin, Global.MissingLogin)
        .AddErrorMessage(AuthFormErrorMessages.MissingReturnUrl, Global.MissingReturnUrl)
        .AddErrorMessage(AuthFormErrorMessages.UserDoesntExist, Global.UserDoesntExist)
        .AddErrorMessage(AuthFormErrorMessages.InvalidCredential, Global.InvalidCredential)
        .AddErrorMessage(AuthFormErrorMessages.UserBlocked, Global.UserAccountIsBlocked)
        .AddErrorMessage(AuthFormErrorMessages.NoActiveOtp, Global.NoActiveOtp)
        .AddErrorMessage(AuthFormErrorMessages.AuthenticationMethodIsNotWellConfigured, Global.AuthenticationMethodIsNotWellConfigured)
        .AddErrorMessage(AuthFormErrorMessages.MissingConfirmationCode, Global.MissingConfirmationCode)
        .AddSuccessMessage(AuthFormSuccessMessages.ConfirmationcodeSent, Global.ConfirmationcodeSent)
        .Build();
}
