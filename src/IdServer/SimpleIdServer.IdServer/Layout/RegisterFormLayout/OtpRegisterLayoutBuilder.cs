// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Layout.RegisterFormLayout;

public class OtpRegisterLayoutBuilder : RegisterLayoutBuilder
{
    private readonly List<LabelTranslation> _valueTranslations;

    protected OtpRegisterLayoutBuilder(FormRecord formRecord, List<LabelTranslation> valueTranslations) : base(formRecord)
    {
        _valueTranslations = valueTranslations;
    }

    public static OtpRegisterLayoutBuilder New(string id, string name, List<LabelTranslation> valueTranslations)
    {
        var record = new FormRecord
        {
            Id = id,
            Name = name,
            ActAsStep = true,
            Elements = new ObservableCollection<IFormElementRecord>()
        };
        return new OtpRegisterLayoutBuilder(record, valueTranslations);
    }

    public OtpRegisterLayoutBuilder ConfigureSendConfirmationCode(string id)
    {
        var elt = new FormStackLayoutRecord
        {
            Id = id,
            IsFormEnabled = true,
            Transformations = StandardRegisterFormComponents.BuildConditionUseToDisplayRegistrationForm(),
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // ReturnUrl.
                StandardFormComponents.NewReturnUrl(),
                // Realm.
                StandardFormComponents.NewRealm(),
                // Action = SENDCONFIRMATIONCODE
                StandardFormComponents.NewOtpSendConfirmationCode(),
                // Value.
                StandardRegisterFormComponents.NewOtpValue(_valueTranslations),
                // Confirmation button.
                StandardFormComponents.NewSendConfirmationCode()
            }
        };
        AddElement(elt);
        return this;
    }

    public OtpRegisterLayoutBuilder ConfigureRegistration(string id)
    {
        var elt = new FormStackLayoutRecord
        {
            Id = id,
            IsFormEnabled = true,
            Transformations = StandardRegisterFormComponents.BuildConditionUseToDisplayRegistrationForm(),
            Elements = new ObservableCollection<IFormElementRecord>
            {                
                // ReturnUrl.
                StandardFormComponents.NewReturnUrl(),
                // Realm.
                StandardFormComponents.NewRealm(),
                // Value
                StandardRegisterFormComponents.NewOtpValueHidden(),
                // Action = REGISTER
                StandardFormComponents.NewOtpRegister(),
                // OTPCode.
                StandardFormComponents.NewOtpCode(),
                // Register button.
                StandardRegisterFormComponents.NewRegister()
            }
        };
        AddElement(elt);
        return this;
    }
}
