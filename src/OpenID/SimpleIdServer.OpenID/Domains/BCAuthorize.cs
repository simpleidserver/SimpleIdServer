// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleIdServer.OpenID.Domains
{
    public class BCAuthorize : ICloneable
    {
        public BCAuthorize()
        {
            Permissions = new List<BCAuthorizePermission>();
        }

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
        public DateTime? RejectionSentDateTime { get; set; }
        public DateTime? NextFetchTime { get; set; }
        public IEnumerable<BCAuthorizePermission> Permissions { get; set; }

        public void Confirm(IEnumerable<string> permissionIds)
        {
            Status = BCAuthorizeStatus.Confirmed;
            UpdateDateTime = DateTime.UtcNow;
            var permissionsToConfirm = Permissions.Where(p => permissionIds.Contains(p.PermissionId));
            foreach(var permission in permissionsToConfirm)
            {
                permission.Confirm();
            }
        }

        public void Pong()
        {
            Status = BCAuthorizeStatus.Pong;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Send()
        {
            Status = BCAuthorizeStatus.Sent;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Reject()
        {
            Status = BCAuthorizeStatus.Rejected;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void NotifyRejection()
        {
            RejectionSentDateTime = DateTime.UtcNow;
        }

        public void IncrementNextFetchTime()
        {
            if (NextFetchTime == null)
            {
                NextFetchTime = DateTime.UtcNow;
            }

            NextFetchTime = NextFetchTime.Value.AddSeconds(Interval);
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
            string notificationToken,
            IEnumerable<BCAuthorizePermission> permissions)
        {
            var result = new BCAuthorize
            {
                Id = GenerateId(),
                ExpirationDateTime = expirationDateTime,
                ClientId = clientId,
                Interval = interval,
                NotificationEdp = notificationEdp,
                NotificationMode = notificationMode,
                Status = BCAuthorizeStatus.Pending,
                Scopes = scopes,
                UserId = userId,
                NotificationToken = notificationToken,
                Permissions = permissions
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
                Permissions = Permissions.Select(p => (BCAuthorizePermission)p.Clone()).ToList(),
                Interval = Interval,
                NextFetchTime = NextFetchTime,
                UpdateDateTime = UpdateDateTime,
                RejectionSentDateTime = RejectionSentDateTime
            };
        }

        private static string GenerateId()
        {
            var builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(160)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }
    }
}
