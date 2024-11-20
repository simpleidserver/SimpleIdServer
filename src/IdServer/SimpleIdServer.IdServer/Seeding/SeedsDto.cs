// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Seeding;

/// <summary>
/// Represents all the entities to seed.
/// </summary>
public class SeedsDto
{
    /// <summary>
    /// Realms to seed.
    /// </summary>
    public IReadOnlyCollection<RealmSeedDto> Realms { get; set; } = [];

    /// <summary>
    /// Scopes to seed.
    /// </summary>
    public IReadOnlyCollection<ScopeSeedDto> Scopes { get; set; } = [];

    /// <summary>
    /// Users to seed.
    /// </summary>
    public IReadOnlyCollection<UserSeedDto> Users { get; set; } = [];
}
