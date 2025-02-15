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

public class MobileRegisterWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Registration;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "mobile",
            WorkflowCorrelationId = "mobileRegisterWorkflow",
            SourceFormCorrelationId = "mobileRegister",
            Links = new List<WorkflowLinkLayout>
            {
                // Register
                new WorkflowLinkLayout
                {
                    Description = "Register",
                    EltCorrelationId = StandardFidoRegisterForms.mobileFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = "{}"
                },                
                // Back
                new WorkflowLinkLayout
                {
                    TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                    Description = "Back",
                    EltCorrelationId = StandardFidoRegisterForms.mobileBackButtonId,
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
