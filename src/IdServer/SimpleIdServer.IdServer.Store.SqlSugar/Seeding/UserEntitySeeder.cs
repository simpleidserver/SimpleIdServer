// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informa
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Seeding;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Seeding;

/// <summary>
/// Implements the method that allows to seed records of users.
/// </summary>
public class UserEntitySeeder : IEntitySeeder<UserSeedDto>
{
    private readonly ILogger<UserEntitySeeder> _logger;
    private readonly DbContext _dbContext;

    public UserEntitySeeder(ILogger<UserEntitySeeder> logger, DbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SeedAsync(IReadOnlyCollection<UserSeedDto> records, CancellationToken cancellationToken = default)
    {
        string[] dbLoginNames = await _dbContext.Client.Queryable<Models.SugarUser>()
            .Select(u => u.Name.ToUpper())
            .ToArrayAsync();

        UserSeedDto[] usersNotInDb = (from r in records
                                      join ln in dbLoginNames on r.Login.ToUpper() equals ln
                                      into qry
                                      from l_join in qry.DefaultIfEmpty()
                                      where l_join == null
                                      select r).ToArray();

        if (usersNotInDb.Length > 0)
        {
            var usersToCreate = new List<Models.SugarUser>();

            foreach (var user in usersNotInDb)
            {
                var builder = UserBuilder.Create(user.Login, user.Password, user.FirstName)
                    .SetLastname(user.LastName)
                    .SetEmail(user.Email);

                foreach (var role in user.Roles) { builder.AddRole(role); }

                Models.SugarUser sugarUser = Models.SugarUser.Transform(builder.Build());

                usersToCreate.Add(sugarUser);
            }

            await _dbContext.Client.Insertable<Models.SugarUser>(usersToCreate).ExecuteCommandAsync(cancellationToken);
        }

        _logger.LogInformation("{count} users seeded.", usersNotInDb.Length);
    }
}
