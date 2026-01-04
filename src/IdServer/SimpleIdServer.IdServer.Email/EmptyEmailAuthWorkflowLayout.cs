// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using FormBuilder;
using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Email.UI.ViewModels;
using SimpleIdServer.IdServer.Layout;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Email;

public class EmptyEmailAuthWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Authentication;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "emptyemail",
            WorkflowCorrelationId = "emptymailAuthWorkflow",
            SourceFormCorrelationId = StandardEmailAuthForms.EmptyEmailForm.CorrelationId,
            Links = new List<WorkflowLinkLayout>
            {
                // Confirmation code.
                new WorkflowLinkLayout
                {
                    EltCorrelationId = StandardEmailAuthForms.emptyEmailSendConfirmationCode,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    IsMainLink = true,
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout
                        {
                            Description = "Confirmation code"
                        }
                    },
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/" + SimpleIdServer.IdServer.Email.Constants.AMR + "/Authenticate",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = $"$.{nameof(AuthenticateEmailViewModel.Realm)}", Target = "realm" }
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
