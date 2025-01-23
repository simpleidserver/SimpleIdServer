// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Back;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Image;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Components.FormElements.Title;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using SimpleIdServer.IdServer.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Layout;

public static class StandardFormComponents
{
    public static IFormElementRecord NewBack()
    {
        return new BackButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = LayoutTranslations.Back
        };
    }

    public static IFormElementRecord NewBackToReturnUrl(string id)
    {
        return new FormAnchorRecord
        {
            Id = id,
            Labels = LayoutTranslations.Back,
            ActAsButton = true
        };
    }

    public static IFormElementRecord NewGenerateQrCode()
    {
        return new FormButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = LayoutTranslations.GenerateQrCode
        };
    }

    public static IFormElementRecord NewScanQrCode()
    {
        return new FormStackLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            IsFormEnabled = false,
            HtmlAttributes = new Dictionary<string, object>
            {
                { "id", "qrCodeContainer" }
            },
            CssStyle = "display: none !important; text-align: center;",
            Elements = new ObservableCollection<IFormElementRecord>
            {
                new TitleRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Labels = LayoutTranslations.ScanQrCode
                },
                new ImageRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    HtmlAttributes = new Dictionary<string, object>
                    {
                        { "id", "qrCode" }
                    },
                    CssStyle = "width:400px",
                    Url = ""
                }
            }
        };
    }

    public static IFormElementRecord NewPassword(string name)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            FormType = FormInputTypes.PASSWORD,
            Labels = LayoutTranslations.Password
        };
    }

    public static IFormElementRecord NewRepeatPassword(string name)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            FormType = FormInputTypes.PASSWORD,
            Labels = LayoutTranslations.RepeatPassword
        };
    }

    public static IFormElementRecord NewAuthenticate()
    {
        return new FormButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = LayoutTranslations.Authenticate
        };
    }

    public static IFormElementRecord NewSendConfirmationCode()
    {
        return new FormButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = LayoutTranslations.SendConfirmationCode
        };
    }

    public static IFormElementRecord NewSeparator()
    {
        return new DividerLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = LayoutTranslations.Separator
        };
    }

    public static IFormElementRecord NewLogin(List<LabelTranslation> translations = null)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(AuthenticatePasswordViewModel.Login),
            Labels = translations ?? LayoutTranslations.Login,
            Transformations = new List<ITransformationRule>
            {
                new IncomingTokensTransformationRule
                {
                    Source = $"$.{nameof(AuthenticatePasswordViewModel.Login)}"
                },
                new PropertyTransformationRule
                {
                    Condition = new LogicalParameter
                    {
                        LeftExpression = new PresentParameter
                        {
                            Source = $"$.{nameof(AuthenticatePasswordViewModel.Login)}"
                        },
                        RightExpression = new UserAuthenticatedParameter(),
                        Operator = LogicalOperators.AND
                    },
                    PropertyName = nameof(FormInputFieldRecord.Disabled),
                    PropertyValue = "true"
                }
            }
        };
    }

    public static IFormElementRecord NewReturnUrl()
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(ISidStepViewModel.ReturnUrl),
            FormType = FormInputTypes.HIDDEN,
            Transformations = new List<ITransformationRule>
            {
                new IncomingTokensTransformationRule
                {
                    Source = $"$.{nameof(ISidStepViewModel.ReturnUrl)}"
                }
            }
        };
    }

    public static IFormElementRecord NewRealm()
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(ISidStepViewModel.Realm),
            FormType = FormInputTypes.HIDDEN,
            Transformations = new List<ITransformationRule>
            {
                new IncomingTokensTransformationRule
                {
                    Source = $"$.{nameof(ISidStepViewModel.Realm)}"
                }
            }
        };
    }

    public static IFormElementRecord NewOtpCode()
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(IOTPViewModel.OTPCode),
            Labels = LayoutTranslations.ConfirmationCode
        };
    }

    public static IFormElementRecord NewOtpSendConfirmationCode()
        => NewOtpAction("SENDCONFIRMATIONCODE");

    public static IFormElementRecord NewOtpAuthenticate()
        => NewOtpAction("AUTHENTICATE");

    public static IFormElementRecord NewOtpRegister()
        => NewOtpAction("REGISTER");

    public static IFormElementRecord NewRememberMe()
        => NewCheckbox(nameof(BaseAuthenticateViewModel.RememberLogin), LayoutTranslations.RememberMe);
    
    public static IFormElementRecord NewHidden(string name)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            FormType = FormInputTypes.HIDDEN
        };
    }

    public static IFormElementRecord NewInput(string name, List<LabelTranslation> translations)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Labels = translations
        };
    }

    public static IFormElementRecord NewTitle(List<LabelTranslation> translations)
    {
        return new TitleRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = translations
        };
    }

    public static IFormElementRecord NewButton(List<LabelTranslation> translations)
    {
        return new FormButtonRecord
        {
            Id = Guid.NewGuid().ToString(),
            Labels = translations
        };
    }

    public static IFormElementRecord NewAnchor(string id, List<LabelTranslation> translations)
    {
        return new FormAnchorRecord
        {
            Id = id,
            Labels = translations
        };
    }

    public static IFormElementRecord NewPassword(string name, List<LabelTranslation> translations)
    {
        return new FormPasswordFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Labels = translations
        };
    }

    public static IFormElementRecord NewCheckbox(string name, List<LabelTranslation> translations)
    {
        return new FormCheckboxRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Value = true,
            Labels = translations
        };
    }

    public static IFormElementRecord NewOtpAction(string actionName)
    {
        return new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameof(IOTPViewModel.Action),
            FormType = FormInputTypes.HIDDEN,
            Transformations = new List<ITransformationRule>
            {
                new StaticValueTransformationRule
                {
                    Value = actionName
                }
            }
        };
    }
}
