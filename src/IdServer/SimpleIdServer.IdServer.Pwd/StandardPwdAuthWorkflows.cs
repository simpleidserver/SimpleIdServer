// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder.Conditions;
using FormBuilder.Link;
using FormBuilder.Models;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Pwd;
using System.Collections.ObjectModel;

namespace FormBuilder.Builders;

public static class StandardPwdAuthWorkflows
{
    public static string pwdWorkflowId = "241a7509-4c58-4f49-b1df-49011b2c9bcb";
    public static string confirmResetPwdWorkflowId = "e05d75d5-5df1-42d4-8c1e-884fc9a2ecff";
    public static string completePwdAuthWorkflowId = "059f49b2-f76a-4b5a-8ecc-cf64abdf9b39";

    public static WorkflowRecord DefaultPwdWorkflow = WorkflowBuilder.New(pwdWorkflowId)
        .AddPwdAuth()
        .AddResetPwd()
        .Build(DateTime.UtcNow);

    public static WorkflowRecord DefaultConfirmResetPwdWorkflow = WorkflowBuilder.New(confirmResetPwdWorkflowId, "resetPwd")
        .AddConfirmResetPwd()
        .Build(DateTime.UtcNow);

    public static WorkflowRecord DefaultCompletePwdAuthWorkflow => WorkflowBuilder.New(completePwdAuthWorkflowId)
        .AddPwdAuth(resetStep: StandardPwdAuthForms.ResetForm)
        .AddResetPwd(StandardPwdAuthForms.ConfirmResetForm)
        .AddConfirmResetPwd()
        .Build(DateTime.UtcNow);

    public static WorkflowBuilder AddPwdAuth(this WorkflowBuilder builder, FormRecord nextStep = null, FormRecord resetStep = null)
    {
        var targets = new List<(FormRecord form, IConditionParameter condition, string description)>
        {
            { (nextStep ?? Constants.EmptyStep, null, "Authenticate") },
            { (StandardPwdAuthForms.ResetTemporaryPasswordForm, new ComparisonParameter
            {
                Operator = ComparisonOperators.EQ,
                Source = "$." + DefaultWorkflowParameters.IsTemporaryCredential,
                Value = "True"
            }, "Reset")}
        };
        builder.AddStep(StandardPwdAuthForms.PwdForm)
            .AddStep(StandardPwdAuthForms.ResetTemporaryPasswordForm)
            .AddLinkHttpRequestAction(StandardPwdAuthForms.PwdForm, targets, StandardPwdAuthForms.pwdAuthFormId, new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/pwd/Authenticate?returnUrl={returnUrl}",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = "$.Realm", Target = "realm" },
                            new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                }
            }, true)
            .AddTransformedLinkUrlAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdAuthExternalIdProviderId, "External auth", "/{realm}/ExternalAuthenticate/Login?scheme={scheme}&returnUrl={returnUrl}&currentLink={currentLink}", new List<ITransformerParameters>
            {
                new RegexTransformerParameters
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = "$.AuthenticationScheme", Target = "scheme" },
                        new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                        new MappingRule { Source = "$.CurrentLink", Target = "currentLink" },
                        new MappingRule { Source = "$.Realm", Target = "realm" }
                    }
                },
                new RelativeUrlTransformerParameters()
            }, false)
            .AddTransformedLinkUrlAction(StandardPwdAuthForms.PwdForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdRegisterBtnId, "Register", "/{realm}/Registration?workflowName=pwd&redirectUrl={returnUrl}", new List<ITransformerParameters>
            {
                new RegexTransformerParameters
                {
                    Rules = new ObservableCollection<MappingRule>
                    {
                        new MappingRule { Source = "$.Realm", Target = "realm" },
                        new MappingRule { Source = "$.AuthUrl", Target = "returnUrl" }
                    }
                },

                new RelativeUrlTransformerParameters()
            }, false)
            .AddLinkHttpRequestAction(StandardPwdAuthForms.PwdForm, resetStep ?? FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdForgetBtnId, "Forget", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.GET,
                Target = "/{realm}/pwd/Reset",
                Transformers = new List<ITransformerParameters>
                {
                    new RegexTransformerParameters()
                    {
                        Rules = new ObservableCollection<MappingRule>
                        {
                            new MappingRule { Source = "$.AuthUrl", Target = "returnUrl" },
                            new MappingRule { Source = "$.Realm", Target = "realm" }
                        }
                    },
                    new RelativeUrlTransformerParameters()
                },
                IsCustomParametersEnabled = true
            }, false)
            .AddLinkHttpRequestAction(StandardPwdAuthForms.ResetTemporaryPasswordForm, Constants.EmptyStep, StandardPwdAuthForms.resetTemporaryPwdFormId, "Authenticate", new WorkflowLinkHttpRequestParameter
            {
                Method = HttpMethods.POST,
                IsAntiforgeryEnabled = true,
                Target = "/{realm}/resetTmpPwd/Authenticate",
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
            }, true);
        return builder;
    }

    public static WorkflowBuilder AddResetPwd(this WorkflowBuilder builder, FormRecord nextStep = null)
    {
        builder.AddStep(StandardPwdAuthForms.ResetForm)
            .AddLinkHttpRequestAction(StandardPwdAuthForms.ResetForm, nextStep ?? FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.pwdResetFormId, "Reset", new WorkflowLinkHttpRequestParameter
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
            }, true);
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
        }, true)
        .AddTransformedLinkUrlAction(StandardPwdAuthForms.ConfirmResetForm, FormBuilder.Constants.EmptyStep, StandardPwdAuthForms.confirmResetPwdBackId, "Back", "{returnUrl}", new List<ITransformerParameters>
        {
            new RegexTransformerParameters
            {
                Rules = new ObservableCollection<MappingRule>
                {
                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" }
                }
            }
        }, false);
        return builder;
    }
}
