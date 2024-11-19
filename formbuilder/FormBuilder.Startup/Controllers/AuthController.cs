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
using FormBuilder.Startup.Controllers.ViewModels;
using FormBuilder.Transformers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FormBuilder.Startup.Controllers;

public class AuthController : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly FormBuilderOptions _options;
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

    public AuthController(IAntiforgery antiforgery, IOptions<FormBuilderOptions> options)
    {
        _antiforgery = antiforgery;
        _options = options.Value;
    }

    public IActionResult Index()
    {
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var viewModel = new AuthViewModel
        {
            ReturnUrl = "http://localhost:5000",
            ExternalIdProviders = new List<ExternalIdProviderViewModel>
            {
                new ExternalIdProviderViewModel { AuthenticationScheme = "facebook", DisplayName = "Facebook" }
            }
        };
        return View(new IndexAuthViewModel
        {
            Form = LoginPwdAuthForm,
            Input = JsonObject.Parse(JsonSerializer.Serialize(viewModel)).AsObject(),
            AntiforgeryToken = new AntiforgeryTokenRecord
            {
                FormValue = tokenSet.RequestToken,
                FormField = tokenSet.FormFieldName,
                CookieName = _options.AntiforgeryCookieName,
                CookieValue = tokenSet.CookieToken
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Confirm(AuthViewModel viewModel)
    {
        return NoContent();
    }

    [HttpGet]
    public IActionResult Callback(string scheme)
    {
        return NoContent();
    }
}
