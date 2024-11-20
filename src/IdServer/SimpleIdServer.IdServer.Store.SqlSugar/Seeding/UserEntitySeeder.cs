// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informa
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Seeding;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;

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
        string[] dbLoginNames = await _dbContext.Client.Queryable<SugarUser>()
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
            var usersToCreate = new List<SugarUser>();

            foreach (UserSeedDto user in usersNotInDb)
            {
                Realm? realm = null;

                if (!string.IsNullOrEmpty(user.Realm))
                {
                    SugarRealm sugarRealm = await _dbContext.Client.Queryable<SugarRealm>()
                        .FirstAsync(r => r.RealmsName.ToUpper() == user.Realm.ToUpper(), cancellationToken);

                    realm = sugarRealm?.ToDomain();
                }

                UserBuilder builder = UserBuilder.Create(user.Login, user.Password, user.FirstName, realm)
                    .SetLastname(user.LastName)
                    .SetEmail(user.Email);

                foreach (string role in user.Roles) { builder.AddRole(role); }

                SugarUser sugarUser = SugarUser.Transform(builder.Build());

                usersToCreate.Add(sugarUser);
            }

            await _dbContext.Client.Insertable<SugarUser>(usersToCreate).ExecuteCommandAsync(cancellationToken);
        }

        _logger.LogInformation("{count} users seeded.", usersNotInDb.Length);
    }
}
