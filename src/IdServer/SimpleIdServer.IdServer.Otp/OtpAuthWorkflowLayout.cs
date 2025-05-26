// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FormBuilder;
using FormBuilder.Link;
using FormBuilder.Models.Layout;
using FormBuilder.Models.Rules;
using FormBuilder.Models.Transformer;
using FormBuilder.Transformers;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Otp.UI.ViewModels;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Otp;

public class OtpAuthWorkflowLayout : IWorkflowLayoutService
{
    public string Category => FormCategories.Authentication;

    public WorkflowLayout Get()
    {
        return new WorkflowLayout
        {
            Name = "otp",
            WorkflowCorrelationId = "otpAuthWorkflow",
            SourceFormCorrelationId = "otpAuth",
            Links = new List<WorkflowLinkLayout>
            {
                // Authenticate.
                new WorkflowLinkLayout
                {
                    Targets = new List<WorkflowLinkTargetLayout>
                    {
                        new WorkflowLinkTargetLayout
                        {
                            Description = "Authenticate"
                        }
                    },
                    EltCorrelationId = StandardOtpAuthForms.otpCodeFormId,
                    ActionType = WorkflowLinkHttpRequestAction.ActionType,
                    IsMainLink = true,
                    ActionParameter = JsonSerializer.Serialize(new WorkflowLinkHttpRequestParameter
                    {
                        Method = HttpMethods.POST,
                        IsAntiforgeryEnabled = true,
                        Target = "/{realm}/" + SimpleIdServer.IdServer.Otp.Constants.Amr + "/Authenticate",
                        Transformers = new List<ITransformerParameters>
                        {
                            new RegexTransformerParameters()
                            {
                                Rules = new ObservableCollection<MappingRule>
                                {
                                    new MappingRule { Source = $"$.{nameof(AuthenticateOtpViewModel.Realm)}", Target = "realm" }
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
