// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Fido;

public static class StandardFidoAuthForms
{
    public static string mobileFormId = "96f59bb9-d01c-4f99-8b40-ddf00c287013";
    public static string webauthnFormId = "aae6a19d-4c09-4099-a425-ce7bb5e4d6dd";

    public static FormRecord MobileForm = QrCodeFormLayoutBuilder.New(Constants.MobileAMR, LayoutTranslations.Login)
        .ConfigureQrCodeGenerationForm(mobileFormId)
        .ConfigureDisplayQrCode()
        .Build();

    public static FormRecord WebauthnForm = AuthLayoutBuilder.New(Constants.AMR)
        .AddElement(new FormStackLayoutRecord
        {
            Id = webauthnFormId,
            IsFormEnabled = true,
            FormType = FormTypes.HTML,
            HtmlAttributes = new Dictionary<string, object>
            {
                { "id", "webauthForm" }
            },
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // Realm
                StandardFormComponents.NewRealm(),
                // SessionId
                StandardFormComponents.NewHidden(nameof(AuthenticateWebauthnViewModel.SessionId)),
                // Login
                StandardFormComponents.NewLogin(),
                // Authenticate
                StandardFormComponents.NewAuthenticate()
            }
        }).Build();
}