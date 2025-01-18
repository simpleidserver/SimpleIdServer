// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;

namespace SimpleIdServer.IdServer.Email;

public static class StandardEmailAuthForms
{
    public static string emailAuthForm = "457701ae-cb96-42d7-ae15-ba83e8eb2193";
    public static string emailSendConfirmationCode = "d472f712-26f6-4e6d-9752-cc9c623c2c69";


    public static FormRecord EmailForm = OtpAuthFormLayoutBuilder.New(Constants.AMR, LayoutTranslations.Email)
        .ConfigureAuthentication(emailAuthForm)
        .ConfigureSendConfirmationCode(emailSendConfirmationCode)
        .Build();
}
