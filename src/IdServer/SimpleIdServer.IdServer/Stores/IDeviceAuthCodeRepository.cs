// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores;

public interface IDeviceAuthCodeRepository
{
    Task<DeviceAuthCode> GetByDeviceCode(string deviceCode, CancellationToken cancellationToken);
    Task<DeviceAuthCode> GetByUserCode(string userCode, CancellationToken cancellationToken);
    void Delete(DeviceAuthCode deviceAuthCode);
    void Add(DeviceAuthCode deviceAuthCode);
    void Update(DeviceAuthCode deviceAuthCode);
}
