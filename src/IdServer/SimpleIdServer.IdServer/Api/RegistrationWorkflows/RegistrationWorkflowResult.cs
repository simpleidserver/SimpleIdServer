// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.RegistrationWorkflows;

public class RegistrationWorkflowResult
{
    [JsonPropertyName(RegistrationWorkflowNames.Id)]
    public string Id { get; set; }
    [JsonPropertyName(RegistrationWorkflowNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(RegistrationWorkflowNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(RegistrationWorkflowNames.UpdateDateTime)]
    public DateTime UpdateDateTime { get; set; }
    [JsonPropertyName(RegistrationWorkflowNames.IsDefault)]
    public bool IsDefault { get; set; }
    [JsonPropertyName(RegistrationWorkflowNames.WorkflowId)]
    public string WorkflowId { get; set; }
}
