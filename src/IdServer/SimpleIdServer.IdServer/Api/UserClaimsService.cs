// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api;

public interface IUserClaimsService
{
    Task<List<UserClaim>> Get(string userId, CancellationToken cancellationToken);
    Task<string> GetUserId(string realm, string claimName, string claimValue, CancellationToken cancellationToken);
}

public class UserClaimsService : IUserClaimsService
{
    private readonly IUserClaimRepository _userClaimRepository;

    public UserClaimsService(IUserClaimRepository userClaimRepository)
    {
        _userClaimRepository = userClaimRepository;
    }

    public async Task<List<UserClaim>> Get(string userId, CancellationToken cancellationToken)
    {
        var claims = await _userClaimRepository.Query().AsNoTracking().Where(c => c.UserId == userId).ToListAsync(cancellationToken);
        return claims;
    }

    public async Task<string> GetUserId(string realm, string claimName, string claimValue, CancellationToken cancellationToken)
    {
        var claim = await _userClaimRepository.Query().Include(c => c.User).ThenInclude(u => u.Realms).AsNoTracking().SingleOrDefaultAsync(c => c.Name == claimName && c.Value == claimValue && c.User.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return claim?.UserId;
    }
}