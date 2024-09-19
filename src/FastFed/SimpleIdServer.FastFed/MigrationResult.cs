// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed;

public class MigrationResult
{
    public int NbMigratedRepresentation { get; set; }
    public List<ProvisioningProfileImportError> Errors { get; set; } = new List<ProvisioningProfileImportError>();
}
