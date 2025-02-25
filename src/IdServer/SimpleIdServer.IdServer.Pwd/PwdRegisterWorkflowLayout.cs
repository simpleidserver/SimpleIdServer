// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Pwd;

public class PwdRegisterWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Registration;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "pwdRegister",
            WorkflowCorrelationId = "registerPwdWorkflow",
            SourceFormCorrelationId = "pwdRegister",
            Links = new List<WorkflowLinkLayout>
            {
                // Register
                new WorkflowLinkLayout
                {
                    Description = "Register",
                    EltCorrelationId = StandardPwdRegisterForms.pwdRegisterFormId,
                    IsMainLink = true,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/pwd/Register",
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
                    EltCorrelationId = StandardPwdRegisterForms.backBtnId,
                    ActionType = WorkflowLinkUrlTransformerAction.ActionType,
                    TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkUrlTransformationParameter
                    {
                        Url = "{returnUrl}",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = $"$.{nameof(IRegisterViewModel.ReturnUrl)}", Target = "returnUrl" }
                                }
                            }
                        }
                    })
                }
            }
        };
    }
}
