﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Domains
{
    public class BCAuthorize : ICloneable
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string NotificationToken { get; set; }
        public string NotificationMode { get; set; }
        public string NotificationEdp { get; set; }
        public int Interval { get; set; }
        public IEnumerable<string> Scopes { get; set; }
        public BCAuthorizeStatus Status { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime NextFetchTime { get; set; }

        public void Confirm()
        {
            Status = BCAuthorizeStatus.Confirmed;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Finish()
        {
            Status = BCAuthorizeStatus.Finished;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void IncrementNextFetchTime()
        {
            NextFetchTime.AddSeconds(Interval);
            UpdateDateTime = DateTime.UtcNow;
        }

        public static BCAuthorize Create(
            DateTime expirationDateTime, 
            string clientId, 
            int interval, 
            string notificationEdp, 
            string notificationMode, 
            IEnumerable<string> scopes,
            string userId,
            string notificationToken)
        {
            var result = new BCAuthorize
            {
                Id = Guid.NewGuid().ToString(),
                ExpirationDateTime = expirationDateTime,
                ClientId = clientId,
                Interval = interval,
                NotificationEdp = notificationEdp,
                NotificationMode = notificationMode,
                Status = BCAuthorizeStatus.Pending,
                Scopes = scopes,
                UserId = userId,
                NotificationToken = notificationToken
            };
            return result;
        }

        public object Clone()
        {
            return new BCAuthorize
            {
                Id = Id,
                ClientId = ClientId,
                UserId = UserId,
                NotificationToken = NotificationToken,
                NotificationMode = NotificationMode,
                NotificationEdp = NotificationEdp,
                Scopes = Scopes,
                Status = Status,
                ExpirationDateTime = ExpirationDateTime,
            };
        }
    }
}
