// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Layout.AuthFormLayout;

public class OtpAuthFormLayoutBuilder
{
    private FormRecord _record;
    private List<LabelTranslation> _loginTranslations;

    private OtpAuthFormLayoutBuilder(FormRecord record, List<LabelTranslation> loginTranslations)
    {
        _record = record;
        _loginTranslations = loginTranslations;
    }

    public static OtpAuthFormLayoutBuilder New(string id, string correlationId, string name, List<LabelTranslation> loginTranslations)
    {
        var record = new FormRecord
        {
            Id = id,
            CorrelationId = correlationId,
            Status = RecordVersionStatus.Published,
            UpdateDateTime = DateTime.UtcNow,
            Name = name,
            ActAsStep = true,
            Realm = Constants.DefaultRealm,
            Category = FormCategories.Authentication,
            Elements = new ObservableCollection<IFormElementRecord>()
        };
        return new OtpAuthFormLayoutBuilder(record, loginTranslations);
    }

    public OtpAuthFormLayoutBuilder AddSuccessMessage(string code, string value)
    {
        _record.SuccessMessageTranslations.Add(new FormMessageTranslation
        {
            Language = Constants.DefaultLanguage,
            Value = value,
            Code = code
        });
        return this;
    }

    public OtpAuthFormLayoutBuilder AddErrorMessage(string code, string value)
    {
        _record.ErrorMessageTranslations.Add(new FormMessageTranslation
        {
            Language = Constants.DefaultLanguage,
            Value = value,
            Code = code
        });
        return this;
    }

    public OtpAuthFormLayoutBuilder ConfigureAuthentication(string id)
    {
        var authForm = new FormStackLayoutRecord
        {
             Id = id,
             CorrelationId = id,
             IsFormEnabled = true,
             Elements = new ObservableCollection<IFormElementRecord>
             {
                 // Return url.
                 StandardFormComponents.NewReturnUrl(),
                 // Realm.
                 StandardFormComponents.NewRealm(),
                 // Action = Authenticate.
                 StandardFormComponents.NewOtpAuthenticate(),
                 // Login
                 StandardFormComponents.NewLogin(_loginTranslations),
                 // OTP.
                 StandardFormComponents.NewOtpCode(),
                 // Authenticate.
                 StandardFormComponents.NewAuthenticate()
             }
        };
        _record.Elements.Add(authForm);
        return this;
    }

    public OtpAuthFormLayoutBuilder ConfigureSendConfirmationCode(string id)
    {
        var sendConfirmationCode = new FormStackLayoutRecord
        {
            Id = id,
            CorrelationId = id,
            IsFormEnabled = true,
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // Return url.
                StandardFormComponents.NewReturnUrl(),
                // Realm.
                StandardFormComponents.NewRealm(),
                // Action = Send confirmation code.
                StandardFormComponents.NewOtpSendConfirmationCode(),
                // Login.
                StandardFormComponents.NewLogin(_loginTranslations),
                // Send confirmation code.
                StandardFormComponents.NewSendConfirmationCode()
            }
        };
        _record.Elements.Add(sendConfirmationCode);
        return this;
    }

    public FormRecord Build() => _record;
}
