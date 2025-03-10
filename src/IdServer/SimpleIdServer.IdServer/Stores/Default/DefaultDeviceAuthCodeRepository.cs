// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultDeviceAuthCodeRepository : IDeviceAuthCodeRepository
{
    private readonly List<DeviceAuthCode> _authCodes;

    public DefaultDeviceAuthCodeRepository(List<DeviceAuthCode> authCodes)
    {
        _authCodes = authCodes;
    }

    public Task<DeviceAuthCode> GetByDeviceCode(string deviceCode, CancellationToken cancellationToken)
    {
        var result = _authCodes.SingleOrDefault(d => d.DeviceCode == deviceCode);
        return Task.FromResult(result);
    }

    public Task<DeviceAuthCode> GetByUserCode(string userCode, CancellationToken cancellationToken)
    {
        var result = _authCodes.SingleOrDefault(d => d.UserCode == userCode);
        return Task.FromResult(result);
    }

    public void Delete(DeviceAuthCode deviceAuthCode) => _authCodes.Remove(deviceAuthCode);

    public void Add(DeviceAuthCode deviceAuthCode) => _authCodes.Add(deviceAuthCode);

    public void Update(DeviceAuthCode deviceAuthCode)
    {
        var index = _authCodes.FindIndex(d => d.DeviceCode == deviceAuthCode.DeviceCode);
        if (index != -1)
            _authCodes[index] = deviceAuthCode;
    }
}
