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
using FormBuilder.Models.Url;
using FormBuilder.Models;
using FormBuilder.Transformers;
using System.Collections.ObjectModel;

namespace FormBuilder.Startup;

public class Constants
{
    public static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Name = "Login and password",
        Elements = new ObservableCollection<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Elements = new ObservableCollection<IFormElementRecord>
                {
                    // Authentication form
                    new FormStackLayoutRecord
                    {
                        IsFormEnabled = true,
                        IsAntiforgeryEnabled = true,
                        Url = new ControllerActionTargetUrl
                        {
                            Action = "Confirm",
                            Controller = "Auth"
                        },
                        Elements = new ObservableCollection<IFormElementRecord>
                        {
                            new FormInputFieldRecord
                            {
                                Name = "ReturnUrl",
                                Value = string.Empty,
                                Type = FormInputTypes.HIDDEN,
                                Transformation = new IncomingTokensTransformationRule
                                {
                                    Source = "$.ReturnUrl"
                                }
                            },
                            new FormInputFieldRecord
                            {
                                Name = "Login",
                                Value = "Login",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build()
                            },
                            new FormPasswordFieldRecord
                            {
                                Name = "Password",
                                Value = "Password",
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Password").Build()
                            },
                            new FormCheckboxRecord
                            {
                                Name = "RememberLogin",
                                Value = true,
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Remember me").Build()
                            },
                            new FormButtonRecord
                            {
                                Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                            }
                        }
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // Forget my password
                    new FormAnchorRecord
                    {
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Forget my password").Build()
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // Register
                    new FormAnchorRecord
                    {
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build(),
                        Url = new DirectTargetUrl { Url = "http://google.com" }
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // List all external identity providers.
                    new ListDataRecord
                    {
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
                            },
                            MappingRules = new List<MappingRule>
                            {
                                new MappingRule { Source = "$.AuthenticationScheme", Target = nameof(FormAnchorRecord.Url), Transformer = new ControllerActionTransformerParameters { Action = "Callback", Controller = "Auth", QueryParameterName = "scheme" } } // Transformer !!!
                            }
                        }
                    }
                }
            }
        }
    };
}
