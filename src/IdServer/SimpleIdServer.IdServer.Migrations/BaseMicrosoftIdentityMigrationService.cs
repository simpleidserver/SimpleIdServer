// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using System.IdentityModel.Tokens.Jwt;

namespace SimpleIdServer.IdServer.Migrations;

public abstract class BaseMicrosoftIdentityMigrationService : IMigrationService
{
    private readonly IApplicationDbContext _dbcontext;

    public BaseMicrosoftIdentityMigrationService(
        IApplicationDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public abstract string Name { get; }

    public abstract Task<int> NbApiScopes(CancellationToken cancellationToken);

    public abstract Task<List<Scope>> ExtractApiScopes(ExtractParameter parameter, CancellationToken cancellationToken);

    public abstract Task<int> NbIdentityScopes(CancellationToken cancellationToken);

    public abstract Task<List<Scope>> ExtractIdentityScopes(ExtractParameter parameter, CancellationToken cancellationToken);

    public abstract Task<int> NbApiResources(CancellationToken cancellationToken);

    public abstract Task<List<ApiResource>> ExtractApiResources(ExtractParameter parameter, CancellationToken cancellationToken);

    public abstract Task<int> NbClients(CancellationToken cancellationToken);

    public abstract Task<List<Client>> ExtractClients(ExtractParameter parameter, CancellationToken cancellationToken);

    public Task<int> NbGroups(CancellationToken cancellationToken)
    {
        return _dbcontext.Roles.CountAsync(cancellationToken);
    }

    public async Task<List<Group>> ExtractGroups(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var allGroups = await _dbcontext.Roles.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(cancellationToken);
        return allGroups.Select(Map).ToList();
    }

    public Task<int> NbUsers(CancellationToken cancellationToken)
    {
        return _dbcontext.Users.CountAsync(cancellationToken);
    }

    public async Task<List<User>> ExtractUsers(ExtractParameter parameter, CancellationToken cancellationToken)
    {
        var users = await _dbcontext.Users.Skip(parameter.StartIndex).Take(parameter.Count).ToListAsync(cancellationToken);
        var allUserIds = users.Select(u => u.Id).ToList();
        var allUserClaims = await _dbcontext.UserClaims.Where(c => allUserIds.Contains(c.UserId)).ToListAsync(cancellationToken);
        var allUserRoles = await _dbcontext.UserRoles.Where(c => allUserIds.Contains(c.UserId)).ToListAsync(cancellationToken);
        var allUserRoleIds = allUserRoles.Select(ur => ur.RoleId).Distinct().ToList();
        var extractedUsers = new List<User>();
        foreach (var user in users)
        {
            var userClaims = allUserClaims.Where(c => c.UserId == user.Id).Select(Map).ToList();
            var userRoles = allUserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
            var extractedUser = Map(user, userClaims, userRoles);
            extractedUsers.Add(extractedUser);
        }

        return extractedUsers;
    }

    private static UserClaim Map(IdentityUserClaim<string> userClaim)
    {
        return new UserClaim
        {
            Id = Guid.NewGuid().ToString(),
            Name = userClaim.ClaimType,
            Value = userClaim.ClaimValue,
            UserId = userClaim.UserId
        };
    }

    private Group Map(IdentityRole identityRole)
    {
        return new Group
        {
            Id = identityRole.Id,
            Name = identityRole.Name,
            Source = Name,
            FullPath = identityRole.Name,
            Description = identityRole.NormalizedName,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
    }


    private User Map(ApplicationUser applicationUser, List<UserClaim> userClaims, List<string> groupIds)
    {
        var result = new User
        {
            Id = applicationUser.Id,
            Source = Name,
            Name = applicationUser.UserName,
            Email = applicationUser.Email,
            UnblockDateTime = applicationUser.LockoutEnd == null ? null : applicationUser.LockoutEnd.Value.UtcDateTime,
            NbLoginAttempt = applicationUser.AccessFailedCount,
            EmailVerified = applicationUser.EmailConfirmed,
            Credentials = new List<UserCredential>
            {
                new UserCredential
                {
                    Id = Guid.NewGuid().ToString(),
                    Value = applicationUser.PasswordHash,
                    CredentialType = UserCredential.PWD,
                    IsActive = true,
                    HashAlg = PasswordHashAlgs.Microsoft
                }
            },
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        result.Status = result.IsBlocked() ? UserStatus.BLOCKED : UserStatus.ACTIVATED;
        var claims = new List<UserClaim>();
        var filteredClaims = userClaims.Where(c => c.UserId == applicationUser.Id);
        claims.AddRange(filteredClaims);
        if (!string.IsNullOrWhiteSpace(applicationUser.PhoneNumber) && !claims.Any(c => c.Type == JwtRegisteredClaimNames.PhoneNumber))
        {
            claims.Add(new UserClaim
            {
                Id = Guid.NewGuid().ToString(),
                Name = JwtRegisteredClaimNames.PhoneNumber,
                Value = applicationUser.PhoneNumber
            });
        }

        if (!claims.Any(c => c.Type == JwtRegisteredClaimNames.PhoneNumberVerified))
        {
            claims.Add(new UserClaim
            {
                Id = Guid.NewGuid().ToString(),
                Name = JwtRegisteredClaimNames.PhoneNumberVerified,
                Value = applicationUser.PhoneNumberConfirmed.ToString().ToLowerInvariant()
            });
        }

        result.OAuthUserClaims = claims;
        result.Groups = groupIds.Select(groupId => new GroupUser
        {
            GroupsId = groupId
        }).ToList();
        return result;
    }
}
