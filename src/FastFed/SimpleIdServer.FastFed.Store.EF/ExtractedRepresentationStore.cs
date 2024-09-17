// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.Store.EF;

public class ExtractedRepresentationStore : IExtractedRepresentationStore
{
    private readonly FastFedDbContext _dbContext;

    public ExtractedRepresentationStore(FastFedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(ExtractedRepresentation representation)
        => _dbContext.ExtractedRepresentations.Add(representation);

    public Task<List<ExtractedRepresentation>> GetRepresentations(int startIndex, CancellationToken cancellationToken)
        => _dbContext.ExtractedRepresentations.OrderBy(p => p.CreateDateTime).Skip(startIndex).ToListAsync(cancellationToken);

    public Task<int> SaveChanges(CancellationToken cancellationToken)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
