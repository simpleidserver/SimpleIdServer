// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
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

    public void Delete(DeviceAuthCode deviceAuthCode) => _dbContext.DeviceAuthCodes.Remove(deviceAuthCode);

    public void Add(DeviceAuthCode deviceAuthCode) => _dbContext.DeviceAuthCodes.Add(deviceAuthCode);

    public Task<int> SaveChanges(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}
