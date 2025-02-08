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
using FormBuilder.Models.Rules;
using FormBuilder.Models;
using FormBuilder.Rules;
using System.Collections.ObjectModel;

namespace FormBuilder.Startup.Workflows;

public class PwdAuthForms
{
    public static string authPwdFormId = "pwdAuth";
    public static string forgetMyPasswordId = "forgetMyPwd";

    public static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Id = Guid.NewGuid().ToString(),
        Name = "pwd",
        CorrelationId = "pwd",
        VersionNumber = 1,
        Status = RecordVersionStatus.Published,
        ActAsStep = true,
        Elements = new ObservableCollection<IFormElementRecord>
        {
        // Authentication form
        new FormStackLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = authPwdFormId,
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
            CorrelationId = Guid.NewGuid().ToString(),
            Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
        },
        // Forget my password
        new FormAnchorRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = forgetMyPasswordId,
            Labels = LabelTranslationBuilder.New().AddTranslation("en", "Forget my password").Build()
        },
        // Separator
        new DividerLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
        },
        // Register
        new FormAnchorRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
        },
        // Separator
        new DividerLayoutRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
        },
        // List all external identity providers.
        new ListDataRecord
        {
            Id = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
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
    };

    public static FormRecord ResetPwdForm = new FormRecord
    {
        Id = "resetPwdId",
        Name = "resetPwd",
        CorrelationId = "resetPwd",
        VersionNumber = 1,
        Status = RecordVersionStatus.Published,
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Id = Guid.NewGuid().ToString(),
                CorrelationId = Guid.NewGuid().ToString(),
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    new FormStackLayoutRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        CorrelationId = Guid.NewGuid().ToString(),
                        IsFormEnabled = true,
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                CorrelationId = Guid.NewGuid().ToString(),
                                Name = "Login",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build()
                            },
                            new FormInputFieldRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                CorrelationId = Guid.NewGuid().ToString(),
                                Name = "Value",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Email").Build()
                            },
                            new FormButtonRecord
                            {
                                Id = Guid.NewGuid().ToString(),
                                CorrelationId = Guid.NewGuid().ToString(),
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Send link").Build()
                            }
                        }
                    }
                }
            }
        }
    };
}