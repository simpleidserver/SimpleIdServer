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

public class ConfirmResetPwdWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Authentication;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "confirmResetPwd",
            WorkflowCorrelationId = "confirmResetPwd",
            SourceFormCorrelationId = "confirmResetPwd",
            Links = new List<WorkflowLinkLayout>
            {
                // Reset
                new WorkflowLinkLayout
                {
                    Description = "Reset",
                    IsMainLink = true,
                    TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                    EltCorrelationId = StandardPwdAuthForms.confirmResetPwdFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
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
                },
                // Back
                new WorkflowLinkLayout
                {
                    Description = "Back",
                    TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                    EltCorrelationId = StandardPwdAuthForms.confirmResetPwdBackId,
                    ActionType = WorkflowLinkUrlTransformerAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter
                    {
                        Url = "{returnUrl}",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = "$.ReturnUrl", Target = "returnUrl" }
                                }
                            }
                        }
                    })
                }
            }
        };
    }
}
