// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;
using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.IdServer.Store.SqlSugar;

public class SerializedFileKeyStore : IFileSerializedKeyStore
{
    private readonly DbContext _dbContext;

    public SerializedFileKeyStore(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<SerializedFileKey>> GetByKeyIds(List<string> keyIds, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarSerializedFileKey>()
        .Includes(s => s.Realms)
            .Where(s => keyIds.Contains(s.KeyId))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public void Add(SerializedFileKey key)
    {
        _dbContext.Client.InsertNav(Transform(key))
            .Include(s => s.Realms)
            .ExecuteCommand();
    }

    public void Update(SerializedFileKey key)
    {
        _dbContext.Client.UpdateNav(Transform(key))
            .Include(s => s.Realms)
            .ExecuteCommand();
    }

    public async Task<List<SerializedFileKey>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarSerializedFileKey>()
            .Includes(s => s.Realms)
            .Where(s => s.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SerializedFileKey>> GetAllEnc(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarSerializedFileKey>()
            .Includes(s => s.Realms)
            .Where(s => s.Usage == DefaultTokenSecurityAlgs.JwkUsages.Enc && s.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SerializedFileKey>> GetAllSig(string realm, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Client.Queryable<SugarSerializedFileKey>()
            .Includes(s => s.Realms)
            .Where(s => s.Usage == DefaultTokenSecurityAlgs.JwkUsages.Sig && s.Realms.Any(r => r.RealmsName == realm))
            .ToListAsync(cancellationToken);
        return result.Select(r => r.ToDomain()).ToList();
    }

    private static SugarSerializedFileKey Transform(SerializedFileKey fileKey)
    {
        return new SugarSerializedFileKey
        {
            Alg = fileKey.Alg,
            CreateDateTime = fileKey.CreateDateTime,
            Enc = fileKey.Enc,
            Id = fileKey.Id,
            IsSymmetric = fileKey.IsSymmetric,
            Key = fileKey.Key ?? new byte[0],
            KeyId = fileKey.KeyId,
            PrivateKeyPem = fileKey.PrivateKeyPem,
            PublicKeyPem = fileKey.PublicKeyPem,
            UpdateDateTime = fileKey.UpdateDateTime,
            Usage = fileKey.Usage,
            Realms = fileKey.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList()
        };
    }
}
