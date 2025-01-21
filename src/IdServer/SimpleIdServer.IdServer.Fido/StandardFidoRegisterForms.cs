// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Fido;

public class StandardFidoRegisterForms
{
    public static string webauthnFormId = "3c5c4862-b03f-4744-935e-2f01724c97f2";
    public static string mobileFormId = "79dccd2d-133c-4749-8225-2b2718337995";

    public static FormRecord WebauthnForm = RegisterLayoutBuilder.New("webauthnRegister", Constants.AMR)
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
                // Realm.
                StandardFormComponents.NewRealm(),
                // Login.
                StandardFormComponents.NewLogin(),
                // DisplayName.
                StandardFormComponents.NewInput(nameof(RegisterWebauthnViewModel.DisplayName), LayoutTranslations.DisplayName),
                // Register btn.
                StandardRegisterFormComponents.NewRegister()
            }
        }).Build();

    public static FormRecord MobileForm = RegisterLayoutBuilder.New("mobileRegister", Constants.MobileAMR)
        .AddElement(new FormStackLayoutRecord
        {
            Id = mobileFormId,
            IsFormEnabled = true,
            FormType = FormTypes.HTML,
            HtmlAttributes = new Dictionary<string, object>
            {
                { "id", "generateQrCodeForm" }
            },
            Elements = new ObservableCollection<IFormElementRecord>
            {                
                // Realm.
                StandardFormComponents.NewRealm(),
                // Login.
                StandardFormComponents.NewLogin(),
                // DisplayName.
                StandardFormComponents.NewInput(nameof(RegisterMobileViewModel.DisplayName), LayoutTranslations.DisplayName),
                // Generate qr code.
                StandardFormComponents.NewGenerateQrCode()
            }
        })
        .AddElement(StandardFormComponents.NewScanQrCode()).Build();
}