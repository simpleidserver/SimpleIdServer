// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;
using SimpleIdServer.IdServer.DTOs;

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
    [JsonPropertyName(RegistrationWorkflowNames.Steps)]
    public List<string> Steps { get; set; } = new List<string>();
    [JsonPropertyName(RegistrationWorkflowNames.IsDefault)]
    public bool IsDefault { get; set; }
}
