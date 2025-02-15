// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
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
                    Description = "Authenticate",
                    EltCorrelationId = StandardPwdAuthForms.pwdAuthFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
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
                },
                // External auth.
                new WorkflowLinkLayout
                {
                    Description = "External auth",
                    EltCorrelationId = StandardPwdAuthForms.pwdAuthExternalIdProviderId,
                    ActionType = WorkflowLinkUrlTransformerAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter 
                    { 
                        Url = "/{realm}/ExternalAuthenticate/Login?scheme={scheme}&returnUrl={returnUrl}", 
                        Transformers = new List<ITransformerParameters>
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
                        }
                    })
                },
                // Register.
                new WorkflowLinkLayout
                {
                    Description = "Register",
                    EltCorrelationId = StandardPwdAuthForms.pwdRegisterBtnId,
                    TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                    ActionType = WorkflowLinkUrlTransformerAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter
                    {
                        Url = "/{realm}/Registration?workflowName=pwd&returnUrl={returnUrl}",
                        Transformers = new List<ITransformerParameters>
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
                        }
                    })
                },
                // Forget
                new WorkflowLinkLayout
                {
                    Description = "Forget",
                    EltCorrelationId = StandardPwdAuthForms.pwdForgetBtnId,
                    TargetFormCorrelationId = StandardPwdAuthForms.ResetForm.CorrelationId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
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
                }
            }
        };
    }
}
