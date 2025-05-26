// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Layout.AuthFormLayout;
using SimpleIdServer.IdServer.Pwd.UI.ViewModels;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Pwd;

public class StandardPwdAuthForms
{
    public static string pwdAuthFormId = "5929ac34-445f-4ebc-819e-d90e4973b30d";
    public static string pwdForgetBtnId = "777b8f76-c7b0-475a-a3c7-5ef0e54ce8e6";
    public static string pwdRegisterBtnId = "7c81db56-24cb-4381-9d77-064424dd65fd";
    public static string pwdAuthExternalIdProviderId = "58cf59f1-e63c-48e3-a0f3-00b0c3d2d38c";

    public static string pwdResetFormId = "8bf5ba00-a9b3-476b-8469-abe123abc797";

    public static string confirmResetPwdFormId = "e42e4c7f-90e8-455d-be48-fbfbc5424f0f";
    public static string confirmResetPwdBackId = "a355339a-d3fb-4a83-90b2-c781c6f0dda4";

    public static string resetTemporaryPwdFormId = "218e85c5-78fa-4b36-a657-d7d906bf2679";

    public static FormRecord PwdForm = AuthLayoutBuilder.New("a415938e-26e1-4065-ac7f-bc583f36b123", "pwdAuth", Constants.AreaPwd)
        .AddElement(new FormStackLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // Authentication form
                new FormStackLayoutRecord
                {
                    Id = pwdAuthFormId,
                    CorrelationId = pwdAuthFormId,
                    IsFormEnabled = true,
                    Elements = new ObservableCollection<IFormElementRecord>
                    {
                        // ReturnUrl.
                        StandardFormComponents.NewReturnUrl(),
                        // Realm.
                        StandardFormComponents.NewRealm(),
                        // Login.
                        StandardFormComponents.NewLogin(),
                        // Password
                        StandardFormComponents.NewPassword(nameof(AuthenticatePasswordViewModel.Password), LayoutTranslations.Password),
                        // Remember me.
                        StandardFormComponents.NewRememberMe(),
                        // Authenticate
                        StandardFormComponents.NewAuthenticate()
                    }
                },
                new FormStackLayoutRecord
                {
                    Transformations = new List<ITransformationRule>
                    {
                        new PropertyTransformationRule
                        {
                            PropertyName = "IsNotVisible",
                            PropertyValue = "true",
                            Condition = new ComparisonParameter
                            {
                                Source = "$.CanResetPassword",
                                Operator = ComparisonOperators.EQ,
                                Value = "false"
                            }
                        },
                        new PropertyTransformationRule
                        {
                            PropertyName = "IsNotVisible",
                            PropertyValue = "false",
                            Condition = new ComparisonParameter
                            {
                                Source = "$.CanResetPassword",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            }
                        }
                    },
                    Elements = new ObservableCollection<IFormElementRecord>
                    {                        
                        // Separator
                        StandardFormComponents.NewSeparator(),
                        // Forget my password
                        StandardFormComponents.NewAnchor(pwdForgetBtnId, LayoutTranslations.ForgetPassword),
                    }
                },
                // Separator
                StandardFormComponents.NewSeparator(),
                // Register                    
                StandardFormComponents.NewAnchor(pwdRegisterBtnId, LayoutTranslations.Register),
                // Separator
                StandardFormComponents.NewSeparator(),
                // List all external identity providers.
                new ListDataRecord
                {
                    Id = pwdAuthExternalIdProviderId,
                    CorrelationId = pwdAuthExternalIdProviderId,
                    FieldType = FormAnchorDefinition.TYPE,
                    Parameters = new Dictionary<string, object>
                    {
                        { nameof(FormAnchorRecord.ActAsButton), true }
                    },
                    RepetitionRule = new IncomingTokensRepetitionRule
                    {
                        Path = $"$.{nameof(BaseAuthenticateViewModel.ExternalIdsProviders)}[*]",
                        MapSameTranslationToAllSupportedLanguages = true,
                        AdditionalInputTokensComingFromStepSource = new List<MappingRule>
                        {
                            new MappingRule { Source = $"$.{nameof(BaseAuthenticateViewModel.Realm)}", Target = "Realm" },
                            new MappingRule { Source = $"$.{nameof(BaseAuthenticateViewModel.ReturnUrl)}", Target = "ReturnUrl" },
                            new MappingRule { Source = $"$.LayoutCurrentLink", Target = "CurrentLink" }
                        },
                        LabelMappingRules = new List<LabelMappingRule>
                        {
                            new LabelMappingRule { Source = $"$.{nameof(ExternalIdProvider.DisplayName)}" },
                        }
                    }
                }
            }
        }).Build();

    public static FormRecord ResetForm = AuthLayoutBuilder.New("8d416c21-2278-4e11-9544-f5a36f979b6d", "resetPwd", "resetPwd", false)
        .AddElement(new FormStackLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            Elements = new ObservableCollection<IFormElementRecord>
            {
                new FormStackLayoutRecord
                {
                    Id = pwdResetFormId,
                    CorrelationId = pwdResetFormId,
                    IsFormEnabled = true,
                    Elements = new ObservableCollection<IFormElementRecord>
                    {
                        // ReturnUrl.
                        StandardFormComponents.NewReturnUrl(),
                        // Realm.
                        StandardFormComponents.NewRealm(),
                        // Reset your password - Title
                        StandardFormComponents.NewTitle(LayoutTranslations.ResetPassword),
                        // Login.
                        StandardFormComponents.NewInput(nameof(ResetPasswordViewModel.Login), LayoutTranslations.Login),
                        // Email.
                        StandardFormComponents.NewInput(nameof(ResetPasswordViewModel.Value), LayoutTranslations.Email),
                        // Send link
                        StandardFormComponents.NewButton(LayoutTranslations.SendLink)
                    }
                },
                // Back button.
                StandardFormComponents.NewBack()
            }
        }).Build();

    public static FormRecord ConfirmResetForm = AuthLayoutBuilder.New("595f4393-ef11-4e59-bae9-3033f945239c", "confirmResetPwd", "confirmResetPwd", false)
        .AddElement(new FormStackLayoutRecord
        {
            Id = confirmResetPwdFormId,
            CorrelationId = confirmResetPwdFormId,
            IsFormEnabled = true,
            Transformations = new List<ITransformationRule>
            {
                new PropertyTransformationRule
                {
                    PropertyName = "IsNotVisible",
                    PropertyValue = "true",
                    Condition = new ComparisonParameter
                    {
                        Source = "$.IsPasswordUpdated",
                        Operator = ComparisonOperators.EQ,
                        Value = "true"
                    }
                }
            },
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // Realm
                StandardFormComponents.NewRealm(),
                // ReturnUrl
                StandardFormComponents.NewReturnUrl(),
                // Login
                new FormInputFieldRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = nameof(ConfirmResetPasswordViewModel.Destination),
                    FormType = FormInputTypes.TEXT,
                    Disabled = true,
                    Transformations = new List<ITransformationRule>
                    {
                        new IncomingTokensTransformationRule
                        {
                            Source = $"$.{nameof(ConfirmResetPasswordViewModel.Destination)}"
                        }
                    },
                    Labels = LayoutTranslations.Login
                },
                // Code
                new FormInputFieldRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Code",
                    FormType = FormInputTypes.TEXT,
                    Disabled = true,
                    Transformations = new List<ITransformationRule>
                    {
                        new IncomingTokensTransformationRule
                        {
                            Source = "$.Code"
                        }
                    },
                    Labels = LayoutTranslations.Code
                },
                // Password
                StandardFormComponents.NewPassword(nameof(ConfirmResetPasswordViewModel.Password)),
                // RepeatPassword
                StandardFormComponents.NewPassword(nameof(ConfirmResetPasswordViewModel.ConfirmationPassword)),
                // Update
                StandardFormComponents.NewButton(LayoutTranslations.Update)
            }
        })
        .AddElement(new FormStackLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            Transformations = new List<ITransformationRule>
            {
                new PropertyTransformationRule
                {
                    PropertyName = nameof(FormStackLayoutRecord.IsNotVisible),
                    PropertyValue = "true",
                    Condition = new ComparisonParameter
                    {
                        Source = $"$.{nameof(ConfirmResetPasswordViewModel.IsPasswordUpdated)}",
                        Operator = ComparisonOperators.EQ,
                        Value = "false"
                    }
                }
            },
            Elements = new ObservableCollection<IFormElementRecord>
            {
                new FormAnchorRecord
                {
                    Id = confirmResetPwdBackId,
                    CorrelationId = confirmResetPwdBackId,
                    ActAsButton = true,
                    Labels = LayoutTranslations.Back
                }
            }
        }).Build();

    public static FormRecord ResetTemporaryPasswordForm = AuthLayoutBuilder.New("d8e6cc6b-cdff-424d-9e09-0daab82a08c2", "resetTmpPwd", "resetTmpPwd", false)
        .AddElement(new FormStackLayoutRecord
        {
            Id = resetTemporaryPwdFormId,
            CorrelationId = resetTemporaryPwdFormId,
            IsFormEnabled = true,
            Elements = new ObservableCollection<IFormElementRecord>
            {
                // Realm
                StandardFormComponents.NewRealm(),
                // ReturnUrl
                StandardFormComponents.NewReturnUrl(),
                // Login
                new FormInputFieldRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = nameof(ResetPasswordIndexViewModel.Login),
                    FormType = FormInputTypes.TEXT,
                    Disabled = true,
                    Transformations = new List<ITransformationRule>
                    {
                        new IncomingTokensTransformationRule
                        {
                            Source = $"$.{nameof(ResetPasswordIndexViewModel.Login)}"
                        }
                    },
                    Labels = LayoutTranslations.Login
                },
                // Password
                StandardFormComponents.NewPassword(nameof(ResetPasswordIndexViewModel.Password)),
                // RepeatPassword
                StandardFormComponents.NewPassword(nameof(ResetPasswordIndexViewModel.ConfirmedPassword)),
                // Update
                StandardFormComponents.NewButton(LayoutTranslations.Update)
            }
        }).Build();
}
