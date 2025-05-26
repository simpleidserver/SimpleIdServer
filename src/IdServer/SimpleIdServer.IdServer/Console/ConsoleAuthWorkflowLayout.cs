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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Console;

public class ConsoleAuthWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Authentication;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "console",
            WorkflowCorrelationId = "consoleAuthWorkflow",
            SourceFormCorrelationId = StandardConsoleAuthForms.ConsoleForm.CorrelationId,
            Links = new List<WorkflowLinkLayout>
            {
                // Confirmation code.
                new WorkflowLinkLayout
                {
                    EltCorrelationId = StandardConsoleAuthForms.consoleSendConfirmationCode,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout 
                        { 
                            TargetFormCorrelationId = FormBuilder.Constants.EmptyStep.CorrelationId,
                            Description = "Confirmation code"
                        }
                    },
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/" + Constants.ConsoleAmr + "/Authenticate",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = $"$.{nameof(AuthenticateConsoleViewModel.Realm)}", Target = "realm" }
                                }
                            },
                            new RelativeUrlTransformerParameters()
                        }
                    })
                },
                // Authenticate
                new WorkflowLinkLayout
                {
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout
                        {
                            Description = "Authenticate"
                        }
                    },
                    IsMainLink = true,
                    EltCorrelationId = StandardConsoleAuthForms.consoleAuthForm,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/" + Constants.ConsoleAmr + "/Authenticate",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = $"$.{nameof(AuthenticateConsoleViewModel.Realm)}", Target = "realm" }
                                }
                            },
                            new RelativeUrlTransformerParameters()
                        }
                    })
                },

            }
        };
    }
}
