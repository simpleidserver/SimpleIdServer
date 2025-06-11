// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Resources;

namespace SimpleIdServer.IdServer.Otp;

public static class StandardOtpAuthForms
{
    public static string otpCodeFormId = "54df94cd-8a59-4a5b-ac5c-fea895f4373f";

    public static FormRecord OtpForm = OtpAuthFormLayoutBuilder.New("934cc5f3-f079-49e7-8a50-0392633df7f7", "otpAuth", Constants.Amr, LayoutTranslations.Login)
        .ConfigureAuthentication(otpCodeFormId)
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
