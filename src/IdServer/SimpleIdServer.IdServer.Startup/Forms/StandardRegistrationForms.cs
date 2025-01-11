// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Image;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Components.FormElements.Title;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Startup.Forms;

public class StandardRegistrationForms
{
    public static string emailSendConfirmationCodeFormId = "7c87cd7b-097c-45b3-8b25-8fd6c0cfef7c";
    public static string emailRegisterFormId = "fb55c356-1555-4b1b-b9fa-05f47c8ad04b";

    public static string smsSendConfirmationCodeFormId = "bf574d6c-dc08-4cb1-a16a-24e596bd4e13";
    public static string smsRegisterFormId = "520cfd8c-4f98-4228-bf6b-b8ff7ba92272";

    public static string pwdRegisterFormId = "e501ca67-2bc9-477e-959c-9752a603fcd1";

    public static string webauthnFormId = "3c5c4862-b03f-4744-935e-2f01724c97f2";

    public static string mobileFormId = "79dccd2d-133c-4749-8225-2b2718337995";

    #region Registration forms

    public static FormRecord EmailForm = new FormRecord
    {
        Id = "6a7629dc-64a1-47b0-9eed-5d3a5998befc",
        Name = "email",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Send confirmation code.
            new FormStackLayoutRecord
            {
                Id = emailSendConfirmationCodeFormId,
                IsFormEnabled = true,
                Transformations = new List<ITransformationRule>
                {
                    new PropertyTransformationRule
                    {
                        PropertyName = "IsNotVisible",
                        PropertyValue = "true",
                        Condition = new LogicalParameter
                        {
                            LeftExpression = new ComparisonParameter
                            {
                                Source = "$.IsCreated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            RightExpression = new ComparisonParameter
                            {
                                Source = "$.IsUpdated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            Operator = LogicalOperators.OR
                        }
                    }
                },
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "RedirectUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.RedirectUrl"
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
                        Name = "Value",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Email").Build(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Value"
                            },
                            new PropertyTransformationRule
                            {
                                Condition = new LogicalParameter
                                {
                                    LeftExpression = new PresentParameter
                                    {
                                        Source = "$.Value"
                                    },
                                    RightExpression = new UserAuthenticatedParameter(),
                                    Operator = LogicalOperators.AND
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
            // Register.
            new FormStackLayoutRecord
            {
                Id = emailRegisterFormId,
                IsFormEnabled = true,
                Transformations = new List<ITransformationRule>
                {
                    new PropertyTransformationRule
                    {
                        PropertyName = "IsNotVisible",
                        PropertyValue = "true",
                        Condition = new LogicalParameter
                        {
                            LeftExpression = new ComparisonParameter
                            {
                                Source = "$.IsCreated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            RightExpression = new ComparisonParameter
                            {
                                Source = "$.IsUpdated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            Operator = LogicalOperators.OR
                        }
                    }
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
                        Name = "Value",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Value"
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
                                Value = "REGISTER"
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
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                    }
                }
            }
        }
    };

    public static FormRecord SmsForm = new FormRecord
    {
        Id = "9bbbe2d0-17fa-4b83-9ea7-16d93f62b9de",
        Name = "sms",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {   
            // Send confirmation code.
            new FormStackLayoutRecord
            {
                Id = smsSendConfirmationCodeFormId,
                IsFormEnabled = true,
                Transformations = new List<ITransformationRule>
                {
                    new PropertyTransformationRule
                    {
                        PropertyName = "IsNotVisible",
                        PropertyValue = "true",
                        Condition = new LogicalParameter
                        {
                            LeftExpression = new ComparisonParameter
                            {
                                Source = "$.IsCreated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            RightExpression = new ComparisonParameter
                            {
                                Source = "$.IsUpdated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            Operator = LogicalOperators.OR
                        }
                    }
                },
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "RedirectUrl",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.RedirectUrl"
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
                        Name = "Value",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Phone number").Build(),
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Value"
                            },
                            new PropertyTransformationRule
                            {
                                Condition = new LogicalParameter
                                {
                                    LeftExpression = new PresentParameter
                                    {
                                        Source = "$.Value"
                                    },
                                    RightExpression = new UserAuthenticatedParameter(),
                                    Operator = LogicalOperators.AND
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
            // Register.
            new FormStackLayoutRecord
            {
                Id = smsRegisterFormId,
                IsFormEnabled = true,
                Transformations = new List<ITransformationRule>
                {
                    new PropertyTransformationRule
                    {
                        PropertyName = "IsNotVisible",
                        PropertyValue = "true",
                        Condition = new LogicalParameter
                        {
                            LeftExpression = new ComparisonParameter
                            {
                                Source = "$.IsCreated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            RightExpression = new ComparisonParameter
                            {
                                Source = "$.IsUpdated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            Operator = LogicalOperators.OR
                        }
                    }
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
                        Name = "Value",
                        FormType = FormInputTypes.HIDDEN,
                        Transformations = new List<ITransformationRule>
                        {
                            new IncomingTokensTransformationRule
                            {
                                Source = "$.Value"
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
                                Value = "REGISTER"
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
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                    }
                }
            }
        }
    };

    public static FormRecord PwdForm = new FormRecord
    {
        Id = "9fee1c1d-2db0-4ea4-b4fa-6366a3928f0e",
        Name = "pwd",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Register.
            new FormStackLayoutRecord
            {
                Id = pwdRegisterFormId,
                IsFormEnabled = true,
                Transformations = new List<ITransformationRule>
                {
                    new PropertyTransformationRule
                    {
                        PropertyName = "IsNotVisible",
                        PropertyValue = "true",
                        Condition = new LogicalParameter
                        {
                            LeftExpression = new ComparisonParameter
                            {
                                Source = "$.IsCreated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            RightExpression = new ComparisonParameter
                            {
                                Source = "$.IsUpdated",
                                Operator = ComparisonOperators.EQ,
                                Value = "true"
                            },
                            Operator = LogicalOperators.OR
                        }
                    }
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
                                Condition = new LogicalParameter
                                {
                                    LeftExpression = new PresentParameter
                                    {
                                        Source = "$.Login"
                                    },
                                    RightExpression = new UserAuthenticatedParameter(),
                                    Operator = LogicalOperators.AND
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Password",
                        FormType = FormInputTypes.PASSWORD,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Password").Build()
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "ConfirmedPassword",
                        FormType = FormInputTypes.PASSWORD,
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Confirmation password").Build()
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                    }
                }
            }
        }
    };

    public static FormRecord WebauthnForm = new FormRecord
    {
        Id = "f8ae1478-0898-4259-b348-b7c158f39fea",
        Name = "webauthn",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
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
                                Condition = new LogicalParameter
                                {
                                    LeftExpression = new PresentParameter
                                    {
                                        Source = "$.Login"
                                    },
                                    RightExpression = new UserAuthenticatedParameter(),
                                    Operator = LogicalOperators.AND
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "DisplayName",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Display name").Build()
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                    }
                }
            }
        }
    };

    public static FormRecord MobileForm = new FormRecord
    {
        Id = "b492a6ec-4edf-4e73-be86-9e0266ecb578",
        Name = "mobile",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            // Generate the QR code.
            new FormStackLayoutRecord
            {
                Id = mobileFormId,
                IsFormEnabled = true,
                FormType = FormTypes.HTML,
                HtmlAttributes = new Dictionary<string, object>
                {
                    { "id", "registerMobile" }
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
                                Condition = new LogicalParameter
                                {
                                    LeftExpression = new PresentParameter
                                    {
                                        Source = "$.Login"
                                    },
                                    RightExpression = new UserAuthenticatedParameter(),
                                    Operator = LogicalOperators.AND
                                },
                                PropertyName = "Disabled",
                                PropertyValue = "true"
                            }
                        }
                    },
                    new FormInputFieldRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "DisplayName",
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Display name").Build()
                    },
                    new FormButtonRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Generate QR code").Build()
                    }
                }
            },
            // Display the QR code.
            new FormStackLayoutRecord
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
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Scan the QR code with your mobile application").Build()
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
            }
        }
    };

    #endregion

    public static List<FormRecord> AllForms => new List<FormRecord>
    {
        EmailForm,
        SmsForm,
        PwdForm,
        WebauthnForm,
        MobileForm
    };
}