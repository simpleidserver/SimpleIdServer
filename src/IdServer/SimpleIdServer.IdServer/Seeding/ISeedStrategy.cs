// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Seeding;

/// <summary>
/// Defines the methods to seed from an external resource.
/// </summary>
public interface ISeedStrategy
{
    /// <summary>
    /// Allows to get all the entities found in the resource.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Entities found.</returns>
    Task<SeedsDto?> GetDataFromResourceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Allows to seed all the entities found in the resource.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task SeedDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Allows to seed all the entities found in the resource.
    /// </summary>
    /// <param name="seeds">The entities to seed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task SeedDataAsync(SeedsDto seeds, CancellationToken cancellationToken = default);
}
