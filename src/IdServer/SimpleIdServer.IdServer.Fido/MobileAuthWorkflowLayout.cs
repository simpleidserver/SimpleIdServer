// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Fido.UI.ViewModels;
using SimpleIdServer.IdServer.Layout;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Fido;

public class MobileAuthWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Authentication;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "mobile",
            WorkflowCorrelationId = "mobileAuthWorkflow",
            SourceFormCorrelationId = "mobileAuth",
            Links = new List<WorkflowLinkLayout>
            {
                // Authenticate
                new WorkflowLinkLayout
                {
                    Description = "Authenticate",
                    EltCorrelationId = StandardFidoAuthForms.mobileFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    IsMainLink = true,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/" + SimpleIdServer.IdServer.Fido.Constants.MobileAMR + "/Authenticate",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = $"$.{nameof(AuthenticateWebauthnViewModel.Realm)}", Target = "realm" }
                                }
                            },
                            new RelativeUrlTransformerParameters()

                        }
                    })
                }
            }
        };
    }
}
