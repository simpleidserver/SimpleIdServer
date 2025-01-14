using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Image;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using System.Collections.ObjectModel;

namespace FormBuilder.Startup;

public class Constants
{
    private static string forgetMyPasswordId = Guid.NewGuid().ToString();
    private static string authPwdFormId = Guid.NewGuid().ToString();
    private static string resetPwdFormId = Guid.NewGuid().ToString();
    private static string registerFormId = Guid.NewGuid().ToString();
    private static string mobileAuthFormId = "mobileAuthFormId";
    private static string loginPwdWorkflowId = "loginPwd";
    private static string mobileWorkflowId = "mobile";
    private static string registerWorkflowId = "register";

    #region Forms

    public static FormRecord ConfirmationForm = new FormRecord
    {
        Id = "confirmationId",
        Name = "Confirmation",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new DividerLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    }
                }
            }
        }
    };

    public static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Id = "pwdId",
        Name = "pwd",
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
                        Id = authPwdFormId,
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
                        Id = forgetMyPasswordId,
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
                            Path = "$.ExternalIdProviders[*]",
                            LabelMappingRules = new List<Rules.LabelMappingRule>
                            {
                                new Rules.LabelMappingRule { Language = "en", Source = "$.DisplayName" }
                            }
                        }
                    }
                }
            }
        }
    };

    public static FormRecord MobileAuthForm = new FormRecord
    {
        Id = "mobileId",
        Name = "mobile",
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = mobileAuthFormId,
                IsFormEnabled = true,
                FormType = FormTypes.HTML,
                HtmlAttributes = new Dictionary<string, object>
                {
                    { "id", "mobileAuthForm" }
                },
                Elements = new ObservableCollection<IFormElementRecord>
                {
                     new FormButtonRecord
                     {
                         Id = Guid.NewGuid().ToString(),
                         Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                     }
                }
            }
        }
    };

    public static FormRecord ViewQrCodeForm = new FormRecord
    {
        Id = "viewQrCodeId",
        Name = "viewQrCode",
        ActAsStep = false,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new ImageRecord
                    {
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

    public static FormRecord ResetLoginPwdForm = new FormRecord
    {
        Id = "resetPwdId",
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
                        Id = resetPwdFormId,
                        IsFormEnabled = true,
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
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

    public static FormRecord RegisterForm = new FormRecord
    {
        Id = "registerId",
        Name = "register",
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormStackLayoutRecord
                    {
                        Id = registerFormId,
                        IsFormEnabled = true,
                        Transformations = new List<ITransformationRule>
                        {
                            new PropertyTransformationRule
                            {
                                PropertyName = "IsNotVisible",
                                PropertyValue = "true",
                                Condition = new ComparisonParameter
                                {
                                    Source = "$.IsRegistered",
                                    Operator = ComparisonOperators.EQ,
                                    Value = "true"
                                }
                            }
                        },
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormButtonRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Create").AddTranslation("en", "Update", new UserAuthenticatedParameter()).Build()
                            }
                        }
                    }
                }
            }
        }
    };

    public static List<FormRecord> AllForms => new List<FormRecord>
    {
        FormBuilder.Constants.EmptyStep,
        LoginPwdAuthForm,
        ConfirmationForm,
        ResetLoginPwdForm,
        MobileAuthForm,
        ViewQrCodeForm,
        RegisterForm
    };

    #endregion

    #region Workflows

    public static WorkflowRecord LoginPwdAuthWorkflow = WorkflowBuilder.New(loginPwdWorkflowId)
        .AddStep(LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(ConfirmationForm, new Coordinate(200, 100))
        .AddLinkPopupAction(LoginPwdAuthForm, ConfirmationForm, forgetMyPasswordId)
        .AddLinkHttpRequestAction(LoginPwdAuthForm, ConfirmationForm, authPwdFormId, new Link.WorkflowLinkHttpRequestParameter
        {
            IsAntiforgeryEnabled = true,
            Target = "http://localhost:62734/Auth"
        })
        .AddLinkUrlAction(LoginPwdAuthForm, ConfirmationForm, "", "")
        .Build();

    public static WorkflowRecord MobileAuthWorkflow = WorkflowBuilder.New(mobileWorkflowId)
        .AddStep(MobileAuthForm, new Coordinate(100, 100))
        .AddStep(ViewQrCodeForm, new Coordinate(200, 100))
        .AddLinkAction(MobileAuthForm, ViewQrCodeForm, mobileAuthFormId)
        .Build();

    public static WorkflowRecord RegisterWorkflow = WorkflowBuilder.New(registerWorkflowId)
        .AddStep(RegisterForm, new Coordinate(100, 100))
        .AddStep(FormBuilder.Constants.EmptyStep, new Coordinate(200, 100))
        .AddLinkHttpRequestAction(RegisterForm, FormBuilder.Constants.EmptyStep, registerFormId, new Link.WorkflowLinkHttpRequestParameter
        {
            IsAntiforgeryEnabled = true,
            Method = Link.HttpMethods.POST,
            Target = "http://localhost:62734/Register"
        })
        .Build();

    public static List<WorkflowRecord> AllWorkflows => new List<WorkflowRecord>
    {
        LoginPwdAuthWorkflow,
        MobileAuthWorkflow,
        RegisterWorkflow
    };

    #endregion
}
