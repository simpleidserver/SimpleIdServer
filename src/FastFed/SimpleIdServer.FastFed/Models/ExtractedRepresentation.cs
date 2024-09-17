// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.FastFed.Models;

public class ExtractedRepresentation
{
    public string Id { get; set; }
    public string SerializedRepresentation {  get; set; }
    public DateTime CreateDateTime { get; set; }
    public string ProvisioningProfileName { get; set; }
}
