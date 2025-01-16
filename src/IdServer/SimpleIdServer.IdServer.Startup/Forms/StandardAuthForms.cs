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

public static class StandardAuthForms
{
    public static string pwdStepId = "pwd";
    public static string resetPwdStepId = "resetPwd";
    public static string consoleStepId = "console";
    public static string emailStepId = "email";
    public static string smsStepId = "sms";
    public static string webAuthnStepId = "webauthn";
    public static string mobileStepId = "mobile";
    public static string otpStepId = "otp";
    public static string displayQrCodeStepId = "displayQrCode";

    public static string pwdAuthFormId = "5929ac34-445f-4ebc-819e-d90e4973b30d";
    public static string pwdAuthExternalIdProviderId = "58cf59f1-e63c-48e3-a0f3-00b0c3d2d38c";
    public static string pwdForgetBtnId = "777b8f76-c7b0-475a-a3c7-5ef0e54ce8e6";
    public static string pwdRegisterBtnId = "7c81db56-24cb-4381-9d77-064424dd65fd";
    public static string pwdResetFormId = "8bf5ba00-a9b3-476b-8469-abe123abc797";

    public static string consoleSendConfirmationCode = "7c07b6f7-f619-4e4f-97d8-3dab508c1c3b";
    public static string consoleAuthForm = "dd9de53a-7165-4019-8073-b5b6476e0892";

    public static string emailSendConfirmationCode = "d472f712-26f6-4e6d-9752-cc9c623c2c69";
    public static string emailAuthForm = "457701ae-cb96-42d7-ae15-ba83e8eb2193";

    public static string smsSendConfirmationCode = "b08e3449-b912-4bf7-9b9d-76a8054e5ff2";
    public static string smsAuthForm = "c7a16cfb-33bf-40c0-a0ce-827ff25e441b";

    public static string webauthnFormId = "aae6a19d-4c09-4099-a425-ce7bb5e4d6dd";

    public static string mobileFormId = "96f59bb9-d01c-4f99-8b40-ddf00c287013";

    public static string displayQrCodeFormId = "e33c2422-cda4-4e44-826e-26eb74ce8bc4";

    public static string otpCodeFormId = "54df94cd-8a59-4a5b-ac5c-fea895f4373f";

    public static string confirmResetPwdFormId = "e42e4c7f-90e8-455d-be48-fbfbc5424f0f";
    public static string confirmResetPwdBackId = "a355339a-d3fb-4a83-90b2-c781c6f0dda4";

    #region Auth forms

    public static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Id = "c6528fa2-59fa-430f-87d1-ba7e0b8fa27c",
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
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").AddTranslation("fr", "Login").Build()
                            },
                            new FormPasswordFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Password",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Password").AddTranslation("fr", "Mot de passe").Build()
                            },
                            new FormCheckboxRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "RememberLogin",
                                Value = true,
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Remember me").AddTranslation("fr", "Se souvenir de moi").Build()
                            },
                            new FormButtonRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").AddTranslation("fr", "Authentifier").Build()
                            }
                        }
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").AddTranslation("fr", "OU").Build()
                    },
                    // Forget my password
                    new FormAnchorRecord
                    {
                        Id = pwdForgetBtnId,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Forget my password").AddTranslation("fr", "Avez-vous oublié votre mot de passe ?").Build()
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").AddTranslation("fr", "OU").Build()
                    },
                    // Register
                    new FormAnchorRecord
                    {
                        Id = pwdRegisterBtnId,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").AddTranslation("fr", "Enregistrer").Build()
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").AddTranslation("fr", "OU").Build()
                    },
                    // List all external identity providers.
                    new ListDataRecord
                    {
                        Id = pwdAuthExternalIdProviderId,
                        FieldType = FormAnchorDefinition.TYPE,
                        Parameters = new Dictionary<string, object>
                        {
                            { nameof(FormAnchorRecord.ActAsButton), true }
                        },
                        RepetitionRule = new IncomingTokensRepetitionRule
                        {
                            Path = "$.ExternalIdsProviders[*]",
                            MapSameTranslationToAllSupportedLanguages = true,
                            AdditionalInputTokensComingFromStepSource = new List<MappingRule>
                            {
                                new MappingRule { Source = "$.Realm", Target = "Realm" },
                                new MappingRule { Source = "$.ReturnUrl", Target = "ReturnUrl" }
                            },
                            LabelMappingRules = new List<FormBuilder.Rules.LabelMappingRule>
                            {
                                new FormBuilder.Rules.LabelMappingRule { Source = "$.DisplayName" },
                            }
                        }
                    }
                }
            }
        }
    };

    public static FormRecord ConsoleAuthForm = new FormRecord
    {
        Id = "14a41d38-0803-4e59-9b6a-a20cf83c1157",
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
        Id = "5ce6d774-fedd-4ada-9818-d327f7406f3c",
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
        Id = "0528a72c-c394-4fb8-a9e6-629307bc7d01",
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
        Id = "8213b4aa-2e4f-49d8-98fe-f09460e1d800",
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

    public static FormRecord MobileForm = new FormRecord
    {
        Id = "d9676620-c98b-45d8-a2a1-ecd3888fcee7",
        Name = mobileStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = mobileFormId,
                IsFormEnabled = true,
                FormType = FormTypes.HTML,
                HtmlAttributes = new Dictionary<string, object>
                {
                    { "id", "mobileForm" }
                },
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

    public static FormRecord DisplayQrCodeForm = new FormRecord
    {
        Id = "1d083e2c-f532-442b-8fc3-8a6d2d6952ac",
        Name = displayQrCodeStepId,
        ActAsStep = false,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = displayQrCodeFormId,
                IsFormEnabled = false,
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new ImageRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Url"
                            }
                        }
                    }
                }
            }
        }
    };

    public static FormRecord OtpForm = new FormRecord
    {
        Id = "d5ed2dc8-80c4-4323-9920-d23f8083f3a8",
        Name = otpStepId,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Authentication.
            new FormStackLayoutRecord
            {
                Id = otpCodeFormId,
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

    #endregion

    #region Reset forms

    public static FormRecord ResetPwdForm = new FormRecord
    {
        Id = "be2b496b-1ab0-47af-b495-133016c40da1",
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

    public static FormRecord ConfirmResetPwdForm = new FormRecord
    {
        Id = "4e2562fb-bb3a-4fd7-953c-c5d6e64bb345",
        Name = "confirmResetPwd",
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Update the pwd.
            new FormStackLayoutRecord
            {
                Id = confirmResetPwdFormId,
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
                    // ReturnUrl
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
                    // Login
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Destination",
                        FormType = FormInputTypes.TEXT,
                        Disabled = true,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Destination"
                            }
                        },
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build()
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
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Code").Build()
                    },
                    // Password
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Password",
                        FormType = FormInputTypes.PASSWORD,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Password").Build()
                    },
                    // RepeatPassword
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ConfirmationPassword",
                        FormType = FormInputTypes.PASSWORD,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Repeat the password").Build()
                    },
                    // Update
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Update").Build()
                    }
                }
            },            
            // Display the back btn.
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
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
                            Value = "false"
                        }
                    }
                },
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormAnchorRecord
                    {
                        Id = confirmResetPwdBackId,
                        ActAsButton = true,
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
        MobileForm,
        DisplayQrCodeForm,
        FormBuilder.Constants.EmptyStep,
        OtpForm,
        ConfirmResetPwdForm
    };
}
