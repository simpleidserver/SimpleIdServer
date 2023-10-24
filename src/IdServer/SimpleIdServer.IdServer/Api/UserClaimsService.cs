// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api;

public interface IUserClaimsService
{
    Task<ICollection<Claim>> Get(string userId, string realm, CancellationToken cancellationToken);
}

public class UserClaimsService : IUserClaimsService
{
    private readonly IUserRepository _userRepository;

    public UserClaimsService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ICollection<Claim>> Get(string userId, string realm, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Query()
            .Include(u => u.Realms)
            .Include(u => u.OAuthUserClaims)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.Realms.Any(r => r.RealmsName == realm), cancellationToken);
        return user?.Claims;
    }
}