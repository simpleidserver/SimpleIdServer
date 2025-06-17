// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Fido;

public static class StandardFidoAuthForms
{
    public static string mobileFormId = "96f59bb9-d01c-4f99-8b40-ddf00c287013";
    public static string webauthnFormId = "aae6a19d-4c09-4099-a425-ce7bb5e4d6dd";

    public static FormRecord MobileForm = QrCodeFormLayoutBuilder.New("afef4e2d-53d3-4eea-9666-965926a87c30", "mobileAuth", Constants.MobileAMR, LayoutTranslations.Login)
        .ConfigureQrCodeGenerationForm(mobileFormId)
        .ConfigureDisplayQrCode()
        .AddErrorMessage(AuthFormErrorMessages.MaximumNumberActiveSessions, Global.MaximumNumberActiveSessions)
        .AddErrorMessage(AuthFormErrorMessages.MissingLogin, Global.MissingLogin)
        .AddErrorMessage(AuthFormErrorMessages.MissingReturnUrl, Global.MissingReturnUrl)
        .AddErrorMessage(AuthFormErrorMessages.MissingSessionId, Global.MissingSessionId)
        .AddErrorMessage(AuthFormErrorMessages.UserDoesntExist, Global.UserDoesntExist)
        .AddErrorMessage(AuthFormErrorMessages.UserBlocked, Global.UserAccountIsBlocked)
        .AddErrorMessage(AuthFormErrorMessages.InvalidCredential, Global.InvalidCredential)
        .AddErrorMessage(AuthFormErrorMessages.UnknownSession, Resources.Global.UnknownSession)
        .AddErrorMessage(AuthFormErrorMessages.SessionNotValidated, Resources.Global.SessionNotValidated)
        .AddErrorMessage(AuthFormErrorMessages.MissingSessionIdParameter, string.Format(Global.MissingParameter, "sessionId"))
        .AddErrorMessage(AuthFormErrorMessages.MissingLoginParameter, string.Format(Global.MissingParameter, "login"))
        .AddErrorMessage(AuthFormErrorMessages.InvalidCaptcha, Global.CaptchaIsNotValid)
        .AddErrorMessage(AuthFormErrorMessages.SessionCannotBeExtracted, Resources.Global.SessionCannotBeExtracted)
        .AddErrorMessage(AuthFormErrorMessages.AuthenticationNotConfirmed, Resources.Global.AuthenticationNotConfirmed)
        .AddErrorMessage(AuthFormErrorMessages.InvalidIncomingRequest, Global.InvalidIncomingRequest)
        .AddErrorMessage(AuthFormErrorMessages.NoCredential, Resources.Global.NoCredential)
        .Build();

    public static FormRecord WebauthnForm = AuthLayoutBuilder.New("90f488c8-c2e6-485f-9825-3b04eca8db07", "webauthnAuth", Constants.AMR)
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
                // ReturnUrl.
                StandardFormComponents.NewReturnUrl(),
                // Realm
                StandardFormComponents.NewRealm(),
                // SessionId
                StandardFormComponents.NewHidden(nameof(IQRCodeAuthViewModel.SessionId)),
                // Login
                StandardFormComponents.NewLogin(),
                // Authenticate
                StandardFormComponents.NewAuthenticate()
            }
        })
        .AddErrorMessage(AuthFormErrorMessages.MaximumNumberActiveSessions, Global.MaximumNumberActiveSessions)
        .AddErrorMessage(AuthFormErrorMessages.MissingLogin, Global.MissingLogin)
        .AddErrorMessage(AuthFormErrorMessages.MissingReturnUrl, Global.MissingReturnUrl)
        .AddErrorMessage(AuthFormErrorMessages.MissingSessionId, Global.MissingSessionId)
        .AddErrorMessage(AuthFormErrorMessages.UserDoesntExist, Global.UserDoesntExist)
        .AddErrorMessage(AuthFormErrorMessages.UserBlocked, Global.UserAccountIsBlocked)
        .AddErrorMessage(AuthFormErrorMessages.InvalidCredential, Global.InvalidCredential)
        .AddErrorMessage(AuthFormErrorMessages.InvalidCaptcha, Global.CaptchaIsNotValid)
        .AddErrorMessage(AuthFormErrorMessages.MissingCredential, Resources.Global.MissingCredential)
        .AddErrorMessage(AuthFormErrorMessages.UnknownSession, Resources.Global.UnknownSession)
        .AddErrorMessage(AuthFormErrorMessages.SessionNotValidated, Resources.Global.SessionNotValidated)
        .AddErrorMessage(AuthFormErrorMessages.MissingSessionIdParameter, string.Format(Global.MissingParameter, "sessionId"))
        .AddErrorMessage(AuthFormErrorMessages.MissingLoginParameter, string.Format(Global.MissingParameter, "login"))
        .AddErrorMessage(AuthFormErrorMessages.SessionCannotBeExtracted, Resources.Global.SessionCannotBeExtracted)
        .AddErrorMessage(AuthFormErrorMessages.AuthenticationNotConfirmed, Resources.Global.AuthenticationNotConfirmed)
        .AddErrorMessage(AuthFormErrorMessages.InvalidIncomingRequest, Global.InvalidIncomingRequest)
        .AddErrorMessage(AuthFormErrorMessages.NoCredential, Resources.Global.NoCredential)
        .Build();
}