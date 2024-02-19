// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
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

    public CustomUserRepository(
        StoreDbContext dbContext, 
        IOptions<UserApiOptions> options) : base(dbContext)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async override Task<User> GetBySubject(string subject, string realm, CancellationToken cancellationToken)
    {
        var result = await GetUsers()
                        .FirstOrDefaultAsync(u => u.Name == subject && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
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
        return base.SaveChanges(cancellationToken);
    }

    private IQueryable<User> GetUsers() => _dbContext.Users
                        .Include(u => u.Consents).ThenInclude(c => c.Scopes).ThenInclude(c => c.AuthorizedResources)
                        .Include(u => u.IdentityProvisioning).ThenInclude(i => i.Definition)
                        .Include(u => u.Credentials)
                        .Include(u => u.ExternalAuthProviders)
                        .Include(u => u.Groups).ThenInclude(u => u.Group)
                        .Include(u => u.Devices)
                        .Include(u => u.Realms);

    record UserCl
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
