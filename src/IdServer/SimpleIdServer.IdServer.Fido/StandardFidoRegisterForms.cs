// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.RegisterFormLayout;
using SimpleIdServer.IdServer.Resources;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Fido;

public class StandardFidoRegisterForms
{
    public static string webauthnFormId = "3c5c4862-b03f-4744-935e-2f01724c97f2";
    public static string mobileFormId = "79dccd2d-133c-4749-8225-2b2718337995";
    public static string webauthnBackButtonId = "2016c54b-d5d6-4649-a169-99aaed300c4e";
    public static string mobileBackButtonId = "39e19212-18a0-47a9-9280-38fb12173d19";

    public static FormRecord WebauthnForm = RegisterLayoutBuilder.New("f74bd92c-a49a-4e28-a4f1-4415e8f24136", "webauthnRegister", Constants.AMR)
        .AddElement(new FormStackLayoutRecord
        {
            Id = webauthnFormId,
            CorrelationId = webauthnFormId,
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
        })
        .ConfigureBackButton(webauthnBackButtonId)
        .AddErrorMessage(RegisterFormErrorMessages.NotAllowedToRegister, Global.NotAllowedToRegister)
        .AddErrorMessage(RegisterFormErrorMessages.MissingSessionIdParameter, string.Format(Global.MissingParameter, "sessionId"))
        .AddErrorMessage(RegisterFormErrorMessages.MissingLoginParameter, string.Format(Global.MissingParameter, "login"))
        .AddErrorMessage(RegisterFormErrorMessages.MissingDisplayNameParameter, string.Format(Global.MissingParameter, "display_name"))
        .AddErrorMessage(RegisterFormErrorMessages.MissingAuthenticatorAttestationRawResponseParameter, string.Format(Global.MissingParameter, "attestation"))
        .AddErrorMessage(RegisterFormErrorMessages.SessionCannotBeExtracted, Resources.Global.SessionCannotBeExtracted)
        .AddErrorMessage(RegisterFormErrorMessages.InvalidIncomingRequest, Global.InvalidIncomingRequest)
        .AddErrorMessage(RegisterFormErrorMessages.RegistrationNotConfirmed, Resources.Global.RegistrationNotConfirmed)
        .AddErrorMessage(RegisterFormErrorMessages.UserWithSameLoginAlreadyExists, Global.UserWithSameLoginAlreadyExists)
        .Build(DateTime.UtcNow);

    public static FormRecord MobileForm = RegisterLayoutBuilder.New("94c14531-9258-40da-8884-2b52fcf9f0f4", "mobileRegister", Constants.MobileAMR)
        .AddElement(new FormStackLayoutRecord
        {
            Id = mobileFormId,
            CorrelationId = mobileFormId,
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
        .AddElement(StandardFormComponents.NewScanQrCode())
        .ConfigureBackButton(mobileBackButtonId)
        .AddErrorMessage(RegisterFormErrorMessages.NotAllowedToRegister, Global.NotAllowedToRegister)
        .AddErrorMessage(RegisterFormErrorMessages.MissingSessionIdParameter, string.Format(Global.MissingParameter, "sessionId"))
        .AddErrorMessage(RegisterFormErrorMessages.MissingLoginParameter, string.Format(Global.MissingParameter, "login"))
        .AddErrorMessage(RegisterFormErrorMessages.MissingDisplayNameParameter, string.Format(Global.MissingParameter, "display_name"))
        .AddErrorMessage(RegisterFormErrorMessages.MissingAuthenticatorAttestationRawResponseParameter, string.Format(Global.MissingParameter, "attestation"))
        .AddErrorMessage(RegisterFormErrorMessages.SessionCannotBeExtracted, Resources.Global.SessionCannotBeExtracted)
        .AddErrorMessage(RegisterFormErrorMessages.InvalidIncomingRequest, Global.InvalidIncomingRequest)
        .AddErrorMessage(RegisterFormErrorMessages.RegistrationNotConfirmed, Resources.Global.RegistrationNotConfirmed)
        .AddErrorMessage(RegisterFormErrorMessages.UserWithSameLoginAlreadyExists, Global.UserWithSameLoginAlreadyExists)
        .Build(DateTime.UtcNow);
}