// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.FastFed.Models;

public class ProvisioningProfileHistory
{
    public string Id { get; set; }
    public string ProfileName { get; set; }
    public int NbMigratedRecords { get; set; }
}
