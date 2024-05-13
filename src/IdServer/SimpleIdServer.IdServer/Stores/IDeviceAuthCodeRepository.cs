// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IDeviceAuthCodeRepository
{
    IQueryable<DeviceAuthCode> Query();
    void Delete(DeviceAuthCode deviceAuthCode);
    void Add(DeviceAuthCode deviceAuthCode);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}
