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

namespace SimpleIdServer.IdServer.Fido;

public class WebauthRegisterWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Registration;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "webauthn",
            WorkflowCorrelationId = "webauthnRegisterWorkflow",
            SourceFormCorrelationId = "webauthnRegister",
            Links = new List<WorkflowLinkLayout>
            {
                // Register
                new WorkflowLinkLayout
                {
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout
                        {
                            Description = "Register"
                        }
                    },
                    EltCorrelationId = StandardFidoRegisterForms.webauthnFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/webauthn/Register",
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
                    Targets = new List<WorkflowLinkTargetLayout> 
                    { 
                        new WorkflowLinkTargetLayout 
                        { 
                            TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                            Description = "Back"
                        } 
                    },
                    EltCorrelationId = StandardFidoRegisterForms.webauthnBackButtonId,
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
