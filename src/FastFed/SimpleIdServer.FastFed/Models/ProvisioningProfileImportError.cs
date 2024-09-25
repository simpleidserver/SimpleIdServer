// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.FastFed.Models;

public class ProvisioningProfileImportError
{
    public string Id { get; set; }
    public string ExtractedRepresentationId { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string ProvisioningProfileHistoryId { get; set; }
    public string EntityId { get; set; }
    public string ProfileName { get; set; }
}
