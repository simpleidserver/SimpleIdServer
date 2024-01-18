// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Domains;

public class ExtractedRepresentationStaging
{
    public string Id { get; set; } = null!;
    public string RepresentationId { get; set; } = null!;
    public string RepresentationVersion { get; set; } = null!;
    public string? Values { get; set; } = null;
    public string IdProvisioningProcessId { get; set; } = null!;
    public List<string> GroupIds { get; set; } = new List<string>();
    public ExtractedRepresentationType Type { get; set; }
}

public enum ExtractedRepresentationType
{
    USER = 0,
    GROUP = 1
}
