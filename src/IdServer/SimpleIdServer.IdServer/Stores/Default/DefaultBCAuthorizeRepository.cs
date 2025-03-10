// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultBCAuthorizeRepository : IBCAuthorizeRepository
{
    private readonly List<BCAuthorize> _bcAuthorizes;

    public DefaultBCAuthorizeRepository(List<BCAuthorize> bcAuthorizes)
    {
        _bcAuthorizes = bcAuthorizes;
    }

    public Task<BCAuthorize> GetById(string id, CancellationToken cancellationToken)
    {
        var result = _bcAuthorizes.SingleOrDefault(b => b.Id == id);
        return Task.FromResult(result);
    }

    public Task<List<BCAuthorize>> GetAllConfirmed(List<string> notificationModes, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var result = _bcAuthorizes
            .Where(a => a.LastStatus == BCAuthorizeStatus.Confirmed &&
                        notificationModes.Contains(a.NotificationMode) &&
                        now < a.ExpirationDateTime)
            .ToList();
        return Task.FromResult(result);
    }

    public void Add(BCAuthorize bcAuthorize) => _bcAuthorizes.Add(bcAuthorize);

    public void Update(BCAuthorize bcAuthorize)
    {
        var existing = _bcAuthorizes.SingleOrDefault(b => b.Id == bcAuthorize.Id);
        if (existing != null)
        {
            var index = _bcAuthorizes.IndexOf(existing);
            _bcAuthorizes[index] = bcAuthorize;
        }
    }
}
