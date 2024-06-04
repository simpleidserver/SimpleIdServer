// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Seeding;

/// <summary>
/// Represents all the entities to seed.
/// </summary>
public class SeedsDto
{
    public IReadOnlyCollection<UserSeedDto> Users { get; set; } = [];
}
