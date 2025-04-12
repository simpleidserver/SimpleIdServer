using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Conditions;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Rules;
using FormBuilder.Tailwindcss;
using System.Collections.ObjectModel;

namespace FormBuilder.Startup.Workflows;

public class PwdAuthForms
{
    public static string authPwdFormId = "pwdAuthId";
    public static string authPwdFormCorrelationId = "pwdAuthCorrelationId";
    public static string forgetMyPasswordId = "forgetMyPwdId";
    public static string forgetMyPasswordCorrelationId = "forgetMyPwdCorrelationId";

    public static FormRecord LoginPwdAuthForm =
        FormRecordBuilder.New("pwd", "pwd", Constants.DefaultRealm, true)
        .SetContainerClass("bg-gray-50 dark:bg-gray-900", TailwindCssTemplate.Name)
        .SetContentClass("flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0", TailwindCssTemplate.Name)
        .SetFormContainerClass("w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700", TailwindCssTemplate.Name)
        .SetFormContentClass("p-6 space-y-4 md:space-y-6 sm:p-8", TailwindCssTemplate.Name)
        .AddStackLayout(authPwdFormId, authPwdFormCorrelationId, (c) =>
        {
            c.EnableForm()
                .AddInputHiddenField("ReturnUrl", new List<ITransformationRule>
                {
                    new IncomingTokensTransformationRule
                    {
                        Source = "$.ReturnUrl"
                    }
                })
                .AddInputTextField("Login", i =>
                {
                    i.AddLabel("fr", "Login")
                    .SetContainerHtmlClass("form-group", TailwindCssTemplate.Name)
                    .SetLabelHtmlClass("block mb-2 text-sm font-medium text-gray-900 dark:text-white", TailwindCssTemplate.Name)
                    .SetTextBoxHtmlClass("bg-gray-50 border border-gray-300 text-gray-900 rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500", TailwindCssTemplate.Name)
                    .SetTransformations(new List<ITransformationRule>
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
                    });
                });
        })
        .AddElement(new FormStackLayoutRecord
        {
            Id = authPwdFormId,
            CorrelationId = authPwdFormCorrelationId,
            IsFormEnabled = true,
            Elements = new ObservableCollection<IFormElementRecord>
            {
                new FormPasswordFieldRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Password",
                    Labels = LabelTranslationBuilder.New().AddTranslation("fr", "Password").Build()
                },
                new FormCheckboxRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "RememberLogin",
                    Labels = LabelTranslationBuilder.New().AddTranslation("fr", "Remember me").Build()
                },
                new FormButtonRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Labels = LabelTranslationBuilder.New().AddTranslation("fr", "Authenticate").Build()
                }
            }
        })
        .AddDivider(c => c.AddTranslation("fr", "OR")
            .SetContainerClass("inline-flex items-center justify-center w-full", TailwindCssTemplate.Name)
            .SetLineClass("w-64 h-px my-8 bg-gray-200 border-0 dark:bg-gray-700", TailwindCssTemplate.Name)
            .SetTextClass("absolute px-3 font-medium text-gray-900 -translate-x-1/2 bg-white left-1/2 dark:text-white dark:bg-gray-800", TailwindCssTemplate.Name))
        .AddAnchor(forgetMyPasswordId, forgetMyPasswordCorrelationId, c => c.AddTranslation("fr", "Forget my password")
            .SetAnchorClass("text-sm font-medium text-primary-600 hover:underline dark:text-primary-500", TailwindCssTemplate.Name))
        .AddDivider(c => c.AddTranslation("fr", "OR")
            .SetContainerClass("inline-flex items-center justify-center w-full", TailwindCssTemplate.Name)
            .SetLineClass("w-64 h-px my-8 bg-gray-200 border-0 dark:bg-gray-700", TailwindCssTemplate.Name)
            .SetTextClass("absolute px-3 font-medium text-gray-900 -translate-x-1/2 bg-white left-1/2 dark:text-white dark:bg-gray-800", TailwindCssTemplate.Name))
        .AddAnchor(cb: c => c.AddTranslation("fr", "Register")
            .SetAnchorClass("text-sm font-medium text-primary-600 hover:underline dark:text-primary-500", TailwindCssTemplate.Name))
        .AddDivider(c => c.AddTranslation("fr", "OR")
            .SetContainerClass("inline-flex items-center justify-center w-full", TailwindCssTemplate.Name)
            .SetLineClass("w-64 h-px my-8 bg-gray-200 border-0 dark:bg-gray-700", TailwindCssTemplate.Name)
            .SetTextClass("absolute px-3 font-medium text-gray-900 -translate-x-1/2 bg-white left-1/2 dark:text-white dark:bg-gray-800", TailwindCssTemplate.Name))
        .Build();

    public static FormRecord ResetPwdForm = new FormRecord
    {
        Id = "resetPwdId",
        Name = "resetPwd",
        CorrelationId = "resetPwd",
        VersionNumber = 0,
        Realm = Constants.DefaultRealm,
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