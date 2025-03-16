// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultFileSerializedKeyStore : IFileSerializedKeyStore
{
    private readonly List<SerializedFileKey> _serializedFileKeys;

    public DefaultFileSerializedKeyStore(List<SerializedFileKey> serializedFileKeys)
    {
        _serializedFileKeys = serializedFileKeys;
    }

    public Task<List<SerializedFileKey>> GetByKeyIds(List<string> keyIds, CancellationToken cancellationToken)
    {
        var result = _serializedFileKeys.Where(s => keyIds.Contains(s.KeyId)).ToList();
        return Task.FromResult(result);
    }

    public IQueryable<SerializedFileKey> Query() => _serializedFileKeys.AsQueryable();

    public Task<List<SerializedFileKey>> GetAll(string realm, CancellationToken cancellationToken)
    {
        var result = _serializedFileKeys.Where(s => s.Realms.Any(r => r.Name == realm)).ToList();
        return Task.FromResult(result);
    }

    public Task<List<SerializedFileKey>> GetAllSig(string realm, CancellationToken cancellationToken)
    {
        var result = _serializedFileKeys.Where(s => s.Usage == DefaultTokenSecurityAlgs.JwkUsages.Sig && s.Realms.Any(r => r.Name == realm)).ToList();
        return Task.FromResult(result);
    }

    public Task<List<SerializedFileKey>> GetAllEnc(string realm, CancellationToken cancellationToken)
    {
        var result = _serializedFileKeys.Where(s => s.Usage == DefaultTokenSecurityAlgs.JwkUsages.Enc && s.Realms.Any(r => r.Name == realm)).ToList();
        return Task.FromResult(result);
    }

    public void Add(SerializedFileKey key) => _serializedFileKeys.Add(key);

    public void Update(SerializedFileKey key)
    {
        var existing = _serializedFileKeys.FirstOrDefault(s => s.Id == key.Id);
        if (existing != null)
        {
            existing.KeyId = key.KeyId;
            existing.Usage = key.Usage;
            existing.Alg = key.Alg;
            existing.Enc = key.Enc;
            existing.PublicKeyPem = key.PublicKeyPem;
            existing.PrivateKeyPem = key.PrivateKeyPem;
            existing.CreateDateTime = key.CreateDateTime;
            existing.UpdateDateTime = key.UpdateDateTime;
            existing.IsSymmetric = key.IsSymmetric;
            existing.Key = key.Key;
            existing.Realms = key.Realms;
        }
    }
}
