// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;

namespace SimpleIdServer.IdServer.Otp;

public static class StandardOtpAuthForms
{
    public static string otpCodeFormId = "54df94cd-8a59-4a5b-ac5c-fea895f4373f";

    public static FormRecord OtpForm = OtpAuthFormLayoutBuilder.New(Constants.Amr, LayoutTranslations.Login)
        .ConfigureAuthentication(otpCodeFormId)
        .Build();
}
