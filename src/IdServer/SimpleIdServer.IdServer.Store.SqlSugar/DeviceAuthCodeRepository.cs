// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class DeviceAuthCodeRepository : IDeviceAuthCodeRepository
    {
        private readonly DbContext _dbContext;

        public DeviceAuthCodeRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(DeviceAuthCode deviceAuthCode)
        {
            _dbContext.Client.Insertable(Transform(deviceAuthCode)).ExecuteCommand();
        }

        public void Delete(DeviceAuthCode deviceAuthCode)
        {
            _dbContext.Client.Deleteable(Transform(deviceAuthCode)).ExecuteCommand();
        }

        public void Update(DeviceAuthCode deviceAuthCode)
        {
            _dbContext.Client.Updateable(Transform(deviceAuthCode)).ExecuteCommand();
        }

        public async Task<DeviceAuthCode> GetByDeviceCode(string deviceCode, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarDeviceAuthCode>()
                .FirstAsync(d => d.DeviceCode == deviceCode, cancellationToken);
            return result?.ToDomain();
        }

        public async Task<DeviceAuthCode> GetByUserCode(string userCode, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarDeviceAuthCode>()
                .FirstAsync(d => d.UserCode == userCode, cancellationToken);
            return result?.ToDomain();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private static SugarDeviceAuthCode Transform(DeviceAuthCode authCode)
        {
            return new SugarDeviceAuthCode
            {
                ClientId = authCode.ClientId,
                CreateDateTime = authCode.CreateDateTime,
                DeviceCode = authCode.DeviceCode,
                ExpirationDateTime = authCode.ExpirationDateTime,
                LastAccessTime = authCode.LastAccessTime,
                NextAccessDateTime = authCode.NextAccessDateTime,
                Status = authCode.Status,
                UpdateDateTime = authCode.UpdateDateTime,
                UserLogin = authCode.UserLogin,
                UserCode = authCode.UserCode,
                Scopes = authCode.Scopes == null ? string.Empty : string.Join(",", authCode.Scopes)
            };
        }
    }
}
