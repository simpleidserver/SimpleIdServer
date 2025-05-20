// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SimpleIdServer.IdServer.Migrations;

public interface IApplicationDbContext
{
    DbSet<IdentityRole> Roles { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<IdentityUserClaim<string>> UserClaims { get; }
    DbSet<IdentityUserRole<string>> UserRoles { get; }
}
