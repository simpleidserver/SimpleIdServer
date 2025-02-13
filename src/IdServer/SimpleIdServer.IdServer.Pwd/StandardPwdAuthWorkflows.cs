// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Pwd;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardPwdAuthWorkflows
{
    public static string pwdWorkflowId = "241a7509-4c58-4f49-b1df-49011b2c9bcb";
    public static string confirmResetPwdWorkflowId = "e05d75d5-5df1-42d4-8c1e-884fc9a2ecff";

    public static WorkflowRecord DefaultPwdWorkflow = WorkflowBuilder.New(pwdWorkflowId)
        .AddPwdAuth()
        .AddResetPwd()
        .Build(DateTime.UtcNow);

    public static WorkflowRecord DefaultConfirmResetPwdWorkflow = WorkflowBuilder.New(confirmResetPwdWorkflowId)
        .AddConfirmResetPwd()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddPwdAuth(this WorkflowBuilder builder, FormRecord nextStep = null, FormRecord resetStep = null)
    {
        builder.AddStep(StandardPwdAuthForms.PwdForm)
            .AddLinkHttpRequestAction(StandardPwdAuthForms.PwdForm, nextStep ?? FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdAuthFormId, "Authenticate", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/pwd/Authenticate",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = "$.Realm", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            })
            .AddTransformedLinkUrlAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdAuthExternalIdProviderId, "External auth", "/{realm}/ExternalAuthenticate/Login?scheme={scheme}&returnUrl={returnUrl}", new List<ITransformerParameters>
            {
                new RegexTransformerParameters
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = "$.AuthenticationScheme", Target = "scheme" },
                        new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                        new MappingRule { Source = "$.Realm", Target = "realm" }
                    }
                },
                new RelativeUrlTransformerParameters()
            })
            .AddTransformedLinkUrlAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdRegisterBtnId, "Register", "/{realm}/Registration?workflowName=pwd&returnUrl={returnUrl}", new List<ITransformerParameters>
            {
                new RegexTransformerParameters
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = "$.Realm", Target = "realm" },
                        new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" }
                    }
                },

                new RelativeUrlTransformerParameters()
            })
            .AddLinkHttpRequestAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdForgetBtnId, "Forget", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.GET,
                Target = "/{realm}/pwd/Reset",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                            new MappingRule { Source = "$.Realm", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                },
                IsCustomParametersEnabled = true,
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.AuthUrl", Target = "returnUrl" }
                },
            })
            .AddLinkHttpRequestAction(StandardPwdAuthForms.ResetForm, resetStep ?? FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdResetFormId, "Reset", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/pwd/Reset",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = "$.Realm", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            });
        return builder;
    }

    public static WorkflowBuilder AddResetPwd(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardPwdAuthForms.ResetForm)
            .AddLinkHttpRequestAction(StandardPwdAuthForms.ResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdResetFormId, "Reset", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/pwd/Reset",
                Transformers = new List<ITransformerParameters>
                    {
                        new RegexTransformerParameters()
                        {
                            Rules = new ObservableCollection<MappingRule>
                            {
                                new MappingRule { Source = "$.Realm", Target = "realm" }
                            }
                        },
                        new RelativeUrlTransformerParameters()
                    }
            });
        return builder;
    }

    public static WorkflowBuilder AddConfirmResetPwd(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardPwdAuthForms.ConfirmResetForm);
        builder.AddLinkHttpRequestAction(StandardPwdAuthForms.ConfirmResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.confirmResetPwdFormId, "Reset", new WorkflowLinkHttpRequestParameter
        {
            Method = HttpMethods.POST,
            IsAntiforgeryEnabled = true,
            Target = "/{realm}/pwd/Reset/Confirm",
            Transformers = new List<ITransformerParameters>
            {
                new RegexTransformerParameters()
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = "$.Realm", Target = "realm" }
                    }
                },
                new RelativeUrlTransformerParameters()
            }
        })
        .AddTransformedLinkUrlAction(StandardPwdAuthForms.ConfirmResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.confirmResetPwdBackId, "Back", "{returnUrl}", new List<ITransformerParameters>
        {
            new RegexTransformerParameters
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" }
                }
            }
        });
        return builder;
    }
}
