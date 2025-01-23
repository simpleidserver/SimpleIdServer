// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;

namespace SimpleIdServer.IdServer.Sms;

public static class StandardSmsRegisterForms
{
    public static string smsSendConfirmationCodeFormId = "bf574d6c-dc08-4cb1-a16a-24e596bd4e13";
    public static string smsRegisterFormId = "520cfd8c-4f98-4228-bf6b-b8ff7ba92272";
    public static string backButtonId = "93aeef90-acd7-4c90-9c13-06c891eec3ee";

    public static FormRecord SmsForm = OtpRegisterLayoutBuilder.New("smsRegister", Constants.AMR, LayoutTranslations.PhoneNumber)
        .ConfigureSendConfirmationCode(smsSendConfirmationCodeFormId)
        .ConfigureRegistration(smsRegisterFormId)
        .ConfigureBackButton(backButtonId)
        .Build();
}
