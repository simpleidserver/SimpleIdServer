using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
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
    private static string workflowId = "loginPwd";

    #region Forms

    public static FormRecord ConfirmationForm = new FormRecord
    {
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

    public static FormRecord ResetLoginPwdForm = new FormRecord
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

    public static List<FormRecord> AllForms => new List<FormRecord>
    {
        LoginPwdAuthForm,
        ConfirmationForm,
        ResetLoginPwdForm
    };

    #endregion

    #region Workflows

    public static WorkflowRecord LoginPwdAuthWorkflow = WorkflowBuilder.New(workflowId)
        .AddStep(LoginPwdAuthForm, new Coordinate(100, 100))
        .AddStep(ConfirmationForm, new Coordinate(200, 100))
        .AddLinkPopupAction(LoginPwdAuthForm, ConfirmationForm, forgetMyPasswordId)
        .AddLinkHttpRequestAction(LoginPwdAuthForm, ConfirmationForm, authPwdFormId, new Link.WorkflowLinkHttpRequestParameter
        {
            IsAntiforgeryEnabled = true,
            Target = "http://localhost:62734/Auth"
        })
        .Build();

    public static List<WorkflowRecord> AllWorkflows => new List<WorkflowRecord>
    {
        LoginPwdAuthWorkflow
    };

    #endregion
}
