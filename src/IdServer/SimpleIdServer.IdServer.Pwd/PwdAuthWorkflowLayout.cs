// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Conditions;
using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Layout;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Pwd;

public class PwdAuthWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Authentication;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "pwd",
            WorkflowCorrelationId = "pwdAuthWorkflow",
            SourceFormCorrelationId = "pwdAuth",
            Links = new List<WorkflowLinkLayout>
            {
                // Authenticate.
                new WorkflowLinkLayout
                {
                    EltCorrelationId = StandardPwdAuthForms.pwdAuthFormId,
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout
                        {
                            Description = "Reset",
                            Condition = new ComparisonParameter
                            {
                                Operator = ComparisonOperators.EQ,
                                Source = "$." + DefaultWorkflowParameters.IsTemporaryCredential,
                                Value = "True"
                            },
                            TargetFormCorrelationId = StandardPwdAuthForms.ResetTemporaryPasswordForm.CorrelationId
                        },
                        new WorkflowLinkTargetLayout
                        {
                            Description = "Authenticate"
                        }
                    },
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    IsMainLink = true,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
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
                    })
                },
                // External auth.
                new WorkflowLinkLayout
                {
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout
                        {
                            Description = "External auth"
                        }
                    },
                    EltCorrelationId = StandardPwdAuthForms.pwdAuthExternalIdProviderId,
                    ActionType = WorkflowLinkUrlTransformerAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter 
                    { 
                        Url = "/{realm}/ExternalAuthenticate/Login?scheme={scheme}&returnUrl={returnUrl}&currentLink={currentLink}", 
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = "$.AuthenticationScheme", Target = "scheme" },
                                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" },
                                    new MappingRule { Source = "$.Realm", Target = "realm" },
                                    new MappingRule { Source = "$.CurrentLink", Target = "currentLink" },
                                }
                            },
                            new RelativeUrlTransformerParameters()
                        }
                    })
                },
                // Register.
                new WorkflowLinkLayout
                {
                    EltCorrelationId = StandardPwdAuthForms.pwdRegisterBtnId,
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout 
                        { 
                            TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                            Description = "Register"
                        }
                    },
                    ActionType = WorkflowLinkUrlTransformerAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter
                    {
                        Url = "/{realm}/Registration?workflowName=pwd&redirectUrl={returnUrl}",
                        Transformers = new List<ITransformerParameters>
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
                        }
                    })
                },
                // Forget
                new WorkflowLinkLayout
                {
                    EltCorrelationId = StandardPwdAuthForms.pwdForgetBtnId,
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout 
                        { 
                            TargetFormCorrelationId = StandardPwdAuthForms.ResetForm.CorrelationId,
                            Description = "Forget"
                        }
                    },
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.GET,
                        Target = "/{realm}/pwd/Reset?returnUrl={returnUrl}",
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
                    })
                }
            }
        };
    }
}
