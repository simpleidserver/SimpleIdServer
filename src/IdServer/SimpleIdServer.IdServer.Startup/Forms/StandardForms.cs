using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Back;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Components.FormElements.Title;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Startup.Forms;

public static class StandardForms
{
    public static string pwdStepId = "pwd";
    public static string resetPwdStepId = "resetPwd";
    public static string consoleStepId = "console";
    public static string emailStepId = "email";
    public static string smsStepId = "sms";
    public static string webAuthnStepId = "webauthn";

    public static string pwdAuthFormId = "5929ac34-445f-4ebc-819e-d90e4973b30d";
    public static string pwdForgetBtnId = "777b8f76-c7b0-475a-a3c7-5ef0e54ce8e6";
    public static string pwdResetFormId = "8bf5ba00-a9b3-476b-8469-abe123abc797";

    public static string consoleSendConfirmationCode = "7c07b6f7-f619-4e4f-97d8-3dab508c1c3b";
    public static string consoleAuthForm = "dd9de53a-7165-4019-8073-b5b6476e0892";

    public static string emailSendConfirmationCode = "d472f712-26f6-4e6d-9752-cc9c623c2c69";
    public static string emailAuthForm = "457701ae-cb96-42d7-ae15-ba83e8eb2193";

    public static string smsSendConfirmationCode = "b08e3449-b912-4bf7-9b9d-76a8054e5ff2";
    public static string smsAuthForm = "c7a16cfb-33bf-40c0-a0ce-827ff25e441b";

    public static string webauthnFormId = "aae6a19d-4c09-4099-a425-ce7bb5e4d6dd";

    #region Auth forms

    public static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Name = pwdStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    // Authentication form
                    new FormStackLayoutRecord
                    {
                        Id = pwdAuthFormId,
                        IsFormEnabled = true,
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "ReturnUrl",
                                FormType = FormInputTypes.HIDDEN,
                                Transformations = new List<ITransformationRule>
                                {
                                    new IncomingTokensTransformationRule
                                    {
                                        Source = "$.ReturnUrl"
                                    }
                                }
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Realm",
                                FormType = FormInputTypes.HIDDEN,
                                Transformations = new List<ITransformationRule>
                                {
                                    new IncomingTokensTransformationRule
                                    {
                                        Source = "$.Realm"
                                    }
                                }
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Login",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build()
                            },
                            new FormPasswordFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Password",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Password").Build()
                            },
                            new FormCheckboxRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "RememberLogin",
                                Value = true,
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Remember me").Build()
                            },
                            new FormButtonRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                            }
                        }
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // Forget my password
                    new FormAnchorRecord
                    {
                        Id = pwdForgetBtnId,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Forget my password").Build()
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // Register
                    new FormAnchorRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // List all external identity providers.
                    new ListDataRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        FieldType = FormAnchorDefinition.TYPE,
                        Parameters = new Dictionary<string, object>
                        {
                            { nameof(FormAnchorRecord.ActAsButton), true }
                        },
                        RepetitionRule = new IncomingTokensRepetitionRule
                        {
                            Path = "$.ExternalIdsProviders[*]",
                            LabelMappingRules = new List<FormBuilder.Rules.LabelMappingRule>
                            {
                                new FormBuilder.Rules.LabelMappingRule { Language = "en", Source = "$.DisplayName" }
                            }
                        }
                    }
                }
            }
        }
    };

    public static FormRecord ConsoleAuthForm = new FormRecord
    {
        Name = consoleStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Send confirmation code.
            new FormStackLayoutRecord
            {
                Id = consoleSendConfirmationCode,
                IsFormEnabled = true,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ReturnUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.ReturnUrl"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new StaticValueTransformationRule
                            {
                                Value = "SENDCONFIRMATIONCODE"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            },
                            new PropertyTransformationRule
                            {
                                Condition = new PresentParameter
                                {
                                    Source = "$.Login"
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Send confirmation code").Build()
                    }
                }
            },
            // Authentication.
            new FormStackLayoutRecord
            {
                Id = consoleAuthForm,
                IsFormEnabled = true,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ReturnUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.ReturnUrl"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new StaticValueTransformationRule
                            {
                                Value = "AUTHENTICATE"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "OTPCode",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Confirmation code").Build()
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                    }
                }
            }
        }
    };

    public static FormRecord EmailAuthForm = new FormRecord
    {
        Name = emailStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Send confirmation code.
            new FormStackLayoutRecord
            {
                Id = emailSendConfirmationCode,
                IsFormEnabled = true,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ReturnUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.ReturnUrl"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new StaticValueTransformationRule
                            {
                                Value = "SENDCONFIRMATIONCODE"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Email").Build(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            },
                            new PropertyTransformationRule
                            {
                                Condition = new PresentParameter
                                {
                                    Source = "$.Login"
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Send confirmation code").Build()
                    }
                }
            },
            // Authentication.
            new FormStackLayoutRecord
            {
                Id = emailAuthForm,
                IsFormEnabled = true,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ReturnUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.ReturnUrl"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new StaticValueTransformationRule
                            {
                                Value = "AUTHENTICATE"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "OTPCode",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Confirmation code").Build()
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                    }
                }
            }
        }
    };

    public static FormRecord SmsAuthForm = new FormRecord
    {
        Name = smsStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Send confirmation code.
            new FormStackLayoutRecord
            {
                Id = smsSendConfirmationCode,
                IsFormEnabled = true,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ReturnUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.ReturnUrl"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new StaticValueTransformationRule
                            {
                                Value = "SENDCONFIRMATIONCODE"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Phone number").Build(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            },
                            new PropertyTransformationRule
                            {
                                Condition = new PresentParameter
                                {
                                    Source = "$.Login"
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Send confirmation code").Build()
                    }
                }
            },
            // Authentication.
            new FormStackLayoutRecord
            {
                Id = smsAuthForm,
                IsFormEnabled = true,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ReturnUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.ReturnUrl"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new StaticValueTransformationRule
                            {
                                Value = "AUTHENTICATE"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "OTPCode",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Confirmation code").Build()
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                    }
                }
            }
        }
    };

    public static FormRecord WebauthnForm = new FormRecord
    {
        Name = webAuthnStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Authentication.
            new FormStackLayoutRecord
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
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Realm",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Realm"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "SessionId",
                        FormType = FormInputTypes.HIDDEN
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Login",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Login"
                            },
                            new PropertyTransformationRule
                            {
                                Condition = new PresentParameter
                                {
                                    Source = "$.Login"
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                    }
                }
            }
        }
    };

    #endregion

    #region Reset forms

    public static FormRecord ResetPwdForm = new FormRecord
    {
        Name = "resetPwd",
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormStackLayoutRecord
                    {
                        Id = pwdResetFormId,
                        IsFormEnabled = true,
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "ReturnUrl",
                                FormType = FormInputTypes.HIDDEN,
                                Transformations = new List<ITransformationRule>
                                {
                                    new IncomingTokensTransformationRule
                                    {
                                        Source = "$.ReturnUrl"
                                    }
                                }
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Realm",
                                FormType = FormInputTypes.HIDDEN,
                                Transformations = new List<ITransformationRule>
                                {
                                    new IncomingTokensTransformationRule
                                    {
                                        Source = "$.Realm"
                                    }
                                }
                            },
                            new TitleRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Reset your password").Build()
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Login",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build()
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Value",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Email").Build()
                            },
                            new FormButtonRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Send link").Build()
                            }
                        }
                    },
                    new BackButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Back").Build()
                    }
                }
            }
        }
    };

    #endregion

    public static List<FormRecord> AllForms => new List<FormRecord>
    {
        LoginPwdAuthForm,
        ResetPwdForm,
        ConsoleAuthForm,
        EmailAuthForm,
        SmsAuthForm,
        WebauthnForm,
        FormBuilder.Constants.EmptyStep
    };
}
