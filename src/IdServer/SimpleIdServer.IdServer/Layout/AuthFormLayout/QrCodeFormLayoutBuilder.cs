// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Layout.AuthFormLayout;

public class QrCodeFormLayoutBuilder
{
    private FormRecord _record;
    private List<LabelTranslation> _loginTranslations;

    private QrCodeFormLayoutBuilder(FormRecord record, List<LabelTranslation> loginTranslations)
    {
        _record = record;
        _loginTranslations = loginTranslations;
    }

    public static QrCodeFormLayoutBuilder New(string id, string correlationId, string name, List<LabelTranslation> loginTranslations)
    {
        var record = new FormRecord
        {
            Id = id,
            CorrelationId = correlationId,
            Name = name,
            ActAsStep = true,
            Elements = new ObservableCollection<IFormElementRecord>()
        };
        return new QrCodeFormLayoutBuilder(record, loginTranslations);
    }

    public QrCodeFormLayoutBuilder ConfigureQrCodeGenerationForm(string id)
    {
        var authForm = new FormStackLayoutRecord
        {
            Id = id,
            IsFormEnabled = true,
            FormType = FormTypes.HTML,
            HtmlAttributes = new Dictionary<string, object>
            {
                { "id", "generateQrCodeForm" }
            },
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // ReturnUrl.
                StandardFormComponents.NewReturnUrl(),
                // Realm.
                StandardFormComponents.NewRealm(),
                // SessionId
                StandardFormComponents.NewHidden(nameof(IQRCodeAuthViewModel.SessionId)),
                // Login.
                StandardFormComponents.NewLogin(_loginTranslations),
                // Generate the qr code.
                StandardFormComponents.NewGenerateQrCode()
            }
        };
        _record.Elements.Add(authForm);
        return this;
    }

    public QrCodeFormLayoutBuilder ConfigureDisplayQrCode()
    {
        var record = StandardFormComponents.NewGenerateQrCode();
        _record.Elements.Add(record);
        return this;
    }

    public FormRecord Build() => _record;
}
