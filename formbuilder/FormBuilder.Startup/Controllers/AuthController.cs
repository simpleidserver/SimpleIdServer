using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.ListData;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Url;
using FormBuilder.Startup.Components;
using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Transformers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FormBuilder.Startup.Controllers;

public class AuthController : Controller
{
    private static FormRecord LoginPwdAuthForm = new FormRecord
    {
        Elements = new List<IFormElementRecord>
        {
            new FormStackLayoutRecord
            {
                Elements = new List<IFormElementRecord>
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
                        Elements = new List<IFormElementRecord>
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
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                    },
                    // Separator
                    new DividerLayoutRecord
                    {
                        Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                    },
                    // List all external identity providers.
                    new ListDataRecord
                    {
                        FieldType = "FormAnchorRecord",
                        Parameters = new Dictionary<string, string>
                        {
                            { nameof(FormAnchorRecord.ActAsButton), "true" }
                        },
                        RepetitionRule = new IncomingTokensRepetitionRule
                        {
                            Path = "$.ExternalIdsProviders",
                            MappingRules = new List<MappingRule>
                            {
                                new MappingRule { Source = "AuthenticationScheme", Target = nameof(FormAnchorRecord.Url), Transformer = new ControllerActionTransformerParameters { Action = "Callback", Controller = "Auth", QueryParameterName = "scheme" } } // Transformer !!!
                            }
                        }
                    }
                }
            }
        }
    };

    [Route("authenticate")]
    public IResult Index()
    {
        var viewModel = new AuthViewModel
        {
            ReturnUrl = "http://localhost:5000",
            ExternalIdProviders = new List<ExternalIdProviderViewModel>
            {
                new ExternalIdProviderViewModel { AuthenticationScheme = "facebook" }
            }
        }; 
        var obj = new { Form = JsonSerializer.Serialize(LoginPwdAuthForm), Input = JsonSerializer.Serialize(viewModel) };
        return new RazorComponentResult<AuthenticateComponent>(obj);
    }

    [Route("authenticate")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirm(AuthViewModel viewModel)
    {
        return NoContent();
    }

    [HttpGet]
    [Route("extauth")]
    public IActionResult Callback(string scheme)
    {
        return NoContent();
    }
}
