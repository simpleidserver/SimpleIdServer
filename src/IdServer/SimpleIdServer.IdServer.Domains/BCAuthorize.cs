// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;

namespace SimpleIdServer.IdServer.Domains
{
    public class BCAuthorize
    {
        public string Id { get; set; } = null!;
        public string? ClientId { get; set; } = null;
        public string? UserId { get; set; } = null;
        public string? NotificationToken { get; set; } = null;
        public string? NotificationMode { get; set; } = null;
        public string? NotificationEdp { get; set; } = null;
        public int? Interval { get; set; } = null;
        public BCAuthorizeStatus Status { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime? RejectionSentDateTime { get; set; }
        public DateTime? NextFetchTime { get; set; }
        public IEnumerable<string> Scopes { get; set; } = new List<string>();

        public void Confirm()
        {
            Status = BCAuthorizeStatus.Confirmed;
            UpdateDateTime = DateTime.UtcNow;
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

            NextFetchTime = NextFetchTime.Value.AddSeconds(Interval.Value);
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
                Id = GenerateId(),
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
