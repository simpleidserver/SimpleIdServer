// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models.Rules;
using FormBuilder.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using FormBuilder.Link;
using FormBuilder.Transformers;
using FormBuilder.Components.FormElements.Title;
using FormBuilder.Components.FormElements.Back;
using FormBuilder.Conditions;
using FormBuilder.Rules;

namespace SimpleIdServer.IdServer.Startup;

public class FormBuilderConfiguration
{
    private static string pwdStepId = "pwd";
    private static string resetPwdStepId = "resetPwd";
    private static string consoleStepId = "console";

    private static string pwdAuthFormId = "5929ac34-445f-4ebc-819e-d90e4973b30d";
    private static string pwdForgetBtnId = "777b8f76-c7b0-475a-a3c7-5ef0e54ce8e6";
    private static string pwdResetFormId = "8bf5ba00-a9b3-476b-8469-abe123abc797";

    private static string consoleSendConfirmationCode = "7c07b6f7-f619-4e4f-97d8-3dab508c1c3b";
    private static string consoleAuthForm = "dd9de53a-7165-4019-8073-b5b6476e0892";

    public static string defaultWorkflowId = "241a7509-4c58-4f49-b1df-49011b2c9bcb";
    public static string pwdConsoleWorkflowId = "e7593fa9-5a73-41a3-bfb5-e489fabbe17a";

    #region Forms

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
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN,
                        // ADD AUTHENTICATE
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
                        Name = "Action",
                        FormType = FormInputTypes.HIDDEN
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

    public static FormRecord ConfirmResetPwdForm = new FormRecord
    {
        Name = "confirmResetPwd"
    };

    public static List<FormRecord> AllForms => new List<FormRecord>
    {
        LoginPwdAuthForm,
        ResetPwdForm,
        ConsoleAuthForm,
        FormBuilder.Constants.EmptyStep
    };

    #endregion

    #region Workflows

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(defaultWorkflowId)
        .AddStep(LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(ResetPwdForm, new Coordinate(200, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(400, 100))
        .AddLinkHttpRequestAction(LoginPwdAuthForm, FormBuilder.Constants.EmptyStep, pwdAuthFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .AddLinkHttpRequestAction(LoginPwdAuthForm, ResetPwdForm, pwdForgetBtnId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.GET,
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            },
            IsCustomParametersEnabled = true,
            Rules = new ObservableCollection<MappingRule>
            {

            },
            Target = "https://localhost:5001/{realm}/pwd/Reset?returnUrl={returnUrl}"
        })
        .AddLinkHttpRequestAction(ResetPwdForm, FormBuilder.Constants.EmptyStep, pwdResetFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Reset",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static WorkflowRecord PwdConsoleWorkflow = WorkflowBuilder.New(pwdConsoleWorkflowId)
        .AddStep(LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(ConsoleAuthForm, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(LoginPwdAuthForm, ConsoleAuthForm, pwdAuthFormId, new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "https://localhost:5001/{realm}/pwd/Authenticate",
            TargetTransformer = new RegexTransformerParameters()
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.Realm", Target = "realm" }
                }
            }
        })
        .Build();

    public static List<WorkflowRecord> AllWorkflows => new List<WorkflowRecord>
    {
        DefaultWorkflow,
        PwdConsoleWorkflow
    };

    #endregion
}
