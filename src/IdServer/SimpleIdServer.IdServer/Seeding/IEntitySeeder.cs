// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Seeding;

/// <summary>
/// Defines the method that allows to seed records of an entity.
/// </summary>
public interface IEntitySeeder<TRecord> where TRecord : class
{
    /// <summary>
    /// Allows to seed records of an entity.
    /// </summary>
    /// <param name="records">Entity records to seed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task SeedAsync(IReadOnlyCollection<TRecord> records, CancellationToken cancellationToken = default);
}
