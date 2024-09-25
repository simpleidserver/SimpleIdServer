// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Stores;

public interface IProvisioningProfileImportErrorStore
{
    void Add(ProvisioningProfileImportError error);
    void Add(List<ProvisioningProfileImportError> errors);
    Task<List<ProvisioningProfileImportError>> GetTop(string entityId, int nb, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
