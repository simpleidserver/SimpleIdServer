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

namespace SimpleIdServer.IdServer.Startup;

public class FormBuilderConfiguration
{
    private static string pwdStepId = "pwd";
    private static string resetPwdStepId = "resetPwd";
    private static string authFormId = "5929ac34-445f-4ebc-819e-d90e4973b30d";
    private static string forgetPwdId = "777b8f76-c7b0-475a-a3c7-5ef0e54ce8e6";
    private static string resetFormId = "8bf5ba00-a9b3-476b-8469-abe123abc797";
    public static string defaultWorkflowId = "241a7509-4c58-4f49-b1df-49011b2c9bcb";

    #region Forms

    public static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Name = "pwd",
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
                        Id = authFormId,
                        IsFormEnabled = true,
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "ReturnUrl",
                                FormType = FormInputTypes.HIDDEN,
                                Transformation = new IncomingTokensTransformationRule
                                {
                                    Source = "$.ReturnUrl"
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
                        Id = forgetPwdId,
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
                        Id = resetFormId,
                        IsFormEnabled = true,
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "ReturnUrl",
                                FormType = FormInputTypes.HIDDEN,
                                Transformation = new IncomingTokensTransformationRule
                                {
                                    Source = "$.ReturnUrl"
                                }
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = "Realm",
                                FormType = FormInputTypes.HIDDEN,
                                Transformation = new IncomingTokensTransformationRule
                                {
                                    Source = "$.Realm"
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
        FormBuilder.Constants.EmptyStep
    };

    #endregion

    #region Workflows

    public static WorkflowRecord DefaultWorkflow = WorkflowBuilder.New(defaultWorkflowId)
        .AddStep(LoginPwdAuthForm, new Coordinate(100, 100), pwdStepId)
        .AddStep(ResetPwdForm, new Coordinate(200, 100), resetPwdStepId)
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(400, 100))
        .AddLinkHttpRequestAction(LoginPwdAuthForm, FormBuilder.Constants.EmptyStep, authFormId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(LoginPwdAuthForm, ResetPwdForm, forgetPwdId, new WorkflowLinkHttpRequestParameter
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
        .AddLinkHttpRequestAction(ResetPwdForm, FormBuilder.Constants.EmptyStep, resetFormId, new WorkflowLinkHttpRequestParameter
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

    public static List<WorkflowRecord> AllWorkflows => new List<WorkflowRecord>
    {
        DefaultWorkflow
    };

    #endregion
}
