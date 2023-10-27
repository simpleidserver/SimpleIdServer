// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Startup.Services;

public class CustomUserRepository : UserRepository
{
    private readonly StoreDbContext _dbContext;
    private readonly UserApiOptions _options;

    public CustomUserRepository(StoreDbContext dbContext, IOptions<UserApiOptions> options) : base(dbContext)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async override Task<User> Get(Func<IQueryable<User>, Task<User>> callback)
    {
        var result = await base.Get(callback);
        if(result != null && result.OAuthUserClaims != null)
        {
            using (var httpClient = new HttpClient())
            {                
                var userClaims = await httpClient.GetFromJsonAsync<IEnumerable<UserCl>>($"{_options.BaseUrl}/users/{result.Id}/claims");
                result.OAuthUserClaims = userClaims.Select(c => new UserClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = result.Id,
                    Name = c.Name,
                    Value = c.Value
                }).ToList();
            }
        }

        return result;
    }

    public override Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        var userClaims = _dbContext.ChangeTracker.Entries().Where(e => e.Entity.GetType() == typeof(UserClaim));
        foreach (var userClaim in userClaims) userClaim.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        return base.SaveChanges(cancellationToken);
    }

    record UserCl
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
