// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Stores;

public interface IExtractedRepresentationStore
{
    void Add(ExtractedRepresentation representation);
    Task<List<ExtractedRepresentation>> GetRepresentations(int startIndex, CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
