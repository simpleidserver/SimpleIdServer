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
        .Build();

    public static WorkflowRecord DefaultConfirmResetPwdWorkflow = WorkflowBuilder.New(confirmResetPwdWorkflowId)
        .AddConfirmResetPwd()
        .Build();

    public static WorkflowBuilder AddPwdAuth(this WorkflowBuilder builder)
    {
        builder.AddStep(StandardPwdAuthForms.PwdForm, new Coordinate(100, 100))
            .AddStep(StandardPwdAuthForms.ResetForm, new Coordinate(100, 100))
            .AddLinkHttpRequestAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
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
            .AddTransformedLinkUrlAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdAuthExternalIdProviderId, "/{realm}/ExternalAuthenticate/Login?scheme={scheme}&returnUrl={returnUrl}", new List<ITransformerParameters>
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
            .AddTransformedLinkUrlAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdRegisterBtnId, "/{realm}/Registration?workflowName=pwd&returnUrl={returnUrl}", new List<ITransformerParameters>
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
            .AddLinkHttpRequestAction(StandardPwdAuthForms.PwdForm, StandardPwdAuthForms.ResetForm, StandardPwdAuthForms.pwdForgetBtnId, new WorkflowLinkHttpRequestParameter
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
            .AddLinkHttpRequestAction(StandardPwdAuthForms.ResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdResetFormId, new WorkflowLinkHttpRequestParameter
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
        builder.AddStep(StandardPwdAuthForms.ConfirmResetForm, new Coordinate(100, 100));
        builder.AddLinkHttpRequestAction(StandardPwdAuthForms.ConfirmResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.confirmResetPwdFormId, new WorkflowLinkHttpRequestParameter
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
        .AddTransformedLinkUrlAction(StandardPwdAuthForms.ConfirmResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.confirmResetPwdBackId, "{returnUrl}", new List<ITransformerParameters>
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
