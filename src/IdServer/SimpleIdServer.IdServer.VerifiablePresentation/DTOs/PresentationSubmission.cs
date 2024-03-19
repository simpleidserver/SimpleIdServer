// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.VerifiablePresentation.DTOs;

public class PresentationSubmission
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("definition_id")]
    public string DefinitionId { get; set; }
    [JsonPropertyName("descriptor_map")]
    public List<PresentationSubmissionDescriptorMap> DescriptorMaps { get; set; }
}