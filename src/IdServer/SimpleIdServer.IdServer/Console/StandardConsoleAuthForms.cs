// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;

namespace SimpleIdServer.IdServer.Console;

public class StandardConsoleAuthForms
{
    public static string consoleSendConfirmationCode = "7c07b6f7-f619-4e4f-97d8-3dab508c1c3b";
    public static string consoleAuthForm = "dd9de53a-7165-4019-8073-b5b6476e0892";

    public static FormRecord ConsoleForm = OtpAuthFormLayoutBuilder.New("88d1d7d8-5b03-4d22-a6f4-9d6a8712fa98", "consoleAuth", Constants.ConsoleAmr, LayoutTranslations.Login)
        .ConfigureSendConfirmationCode(consoleSendConfirmationCode)
        .ConfigureAuthentication(consoleAuthForm)
        .Build();
}