// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SimpleIdServer.IdServer.Startup.Forms;

public class StandardRegistrationForms
{
    public static string emailSendConfirmationCodeFormId = "7c87cd7b-097c-45b3-8b25-8fd6c0cfef7c";
    public static string emailRegisterFormId = "fb55c356-1555-4b1b-b9fa-05f47c8ad04b";

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

    #endregion

    public static List<FormRecord> AllForms => new List<FormRecord>
    {
        EmailForm
    };
}