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

namespace SimpleIdServer.IdServer.VerifiablePresentation;

internal class VpRegisterWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Registration;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            WorkflowCorrelationId = "vpRegistrationWorkflow",
            SourceFormCorrelationId = "vpRegister",
            Links = new List<WorkflowLinkLayout>
            {
                // Choose vp.
                new WorkflowLinkLayout
                {
                    Description = "Choose vp",
                    EltCorrelationId = StandardVpRegisterForms.vpRegistrationFormId,
                    ActionType = WorkflowLinkAction.ActionType
                },
                // Back
                new WorkflowLinkLayout
                {
                    Description = "Back",
                    EltCorrelationId = StandardVpRegisterForms.backBtnId,
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
