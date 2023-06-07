// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains
{
    public class DeviceAuthCode
    {
        public string DeviceCode { get; set; } = null!;
        public string UserCode { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string? UserLogin { get; set; } = null;
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime? NextAccessDateTime { get; set; } = null;
        public DeviceAuthCodeStatus Status { get; set; } = DeviceAuthCodeStatus.PENDING;
        public DateTime LastAccessTime { get; set; }
        public Client Client { get; set; } = null!;
        public User? User { get; set; } = null;

        public void Send()
        {
            Status = DeviceAuthCodeStatus.ISSUED;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Accept(string userLogin)
        {
            Status = DeviceAuthCodeStatus.ACCEPTED;
            UpdateDateTime = DateTime.UtcNow;
            UserLogin = userLogin;
        }

        public void Next(double slidingTimeInSeconds)
        {
            DateTime.UtcNow.AddSeconds(slidingTimeInSeconds);
            UpdateDateTime = DateTime.UtcNow;
        }

        public static DeviceAuthCode Create(string deviceCode, string userCode, string clientId, IEnumerable<string> scopes, double slidingExpirationTimesInSeconds)
        {
            return new DeviceAuthCode
            {
                ClientId = clientId,
                DeviceCode = deviceCode,
                Scopes = scopes,
                UserCode = userCode,
                ExpirationDateTime = DateTime.UtcNow.AddSeconds(slidingExpirationTimesInSeconds),
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }
    }

    public enum DeviceAuthCodeStatus
    {
        PENDING = 0,
        ACCEPTED = 1,
        ISSUED = 2
    }
}
