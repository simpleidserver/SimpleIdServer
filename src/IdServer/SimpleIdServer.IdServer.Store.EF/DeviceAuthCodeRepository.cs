// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.EF;

public class DeviceAuthCodeRepository : IDeviceAuthCodeRepository
{
    private readonly StoreDbContext _dbContext;

    public DeviceAuthCodeRepository(StoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<DeviceAuthCode> Query() => _dbContext.DeviceAuthCodes;

    public Task<DeviceAuthCode> GetByDeviceCode(string deviceCode, CancellationToken cancellationToken)
    {
        return _dbContext.DeviceAuthCodes.SingleOrDefaultAsync(d => d.DeviceCode == deviceCode, cancellationToken);
    }

    public Task<DeviceAuthCode> GetByUserCode(string userCode, CancellationToken cancellationToken)
    {
        return _dbContext.DeviceAuthCodes.Include(c => c.Client).ThenInclude(c => c.Scopes).SingleOrDefaultAsync(d => d.UserCode == userCode, cancellationToken);
    }

    public void Delete(DeviceAuthCode deviceAuthCode) => _dbContext.DeviceAuthCodes.Remove(deviceAuthCode);

    public void Add(DeviceAuthCode deviceAuthCode) => _dbContext.DeviceAuthCodes.Add(deviceAuthCode);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
