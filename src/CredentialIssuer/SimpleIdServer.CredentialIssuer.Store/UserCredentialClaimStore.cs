// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.CredentialIssuer.Domains;

namespace SimpleIdServer.CredentialIssuer.Store;

public interface IUserCredentialClaimStore
{
    Task<List<UserCredentialClaim>> Resolve(string subject, List<CredentialConfigurationClaim> claims, CancellationToken cancellationToken);
}

public class UserCredentialClaimStore : IUserCredentialClaimStore
{
    private readonly CredentialIssuerDbContext _dbContext;

    public UserCredentialClaimStore(CredentialIssuerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<UserCredentialClaim>> Resolve(string subject, List<CredentialConfigurationClaim> claims, CancellationToken cancellationToken)
    {
        var claimNames = claims.Select(c => c.SourceUserClaimName);
        return _dbContext.UserCredentialClaims.Where(c => c.Subject == subject && claimNames.Contains(c.Name)).ToListAsync(cancellationToken);
    }
}