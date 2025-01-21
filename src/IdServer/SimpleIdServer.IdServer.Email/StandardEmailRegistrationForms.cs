// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;

namespace SimpleIdServer.IdServer.Email;

public class StandardEmailRegistrationForms
{
    public static string emailSendConfirmationCodeFormId = "7c87cd7b-097c-45b3-8b25-8fd6c0cfef7c";
    public static string emailRegisterFormId = "fb55c356-1555-4b1b-b9fa-05f47c8ad04b";

    public static FormRecord EmailForm = OtpRegisterLayoutBuilder.New("emailRegister", Constants.AMR, LayoutTranslations.Email)
        .ConfigureRegistration(emailRegisterFormId)
        .ConfigureSendConfirmationCode(emailSendConfirmationCodeFormId)
        .Build();
}