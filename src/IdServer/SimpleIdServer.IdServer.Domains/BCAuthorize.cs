// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SimpleIdServer.IdServer.Domains
{
    public class BCAuthorize
    {
        public string Id { get; set; } = null!;
        public string? ClientId { get; set; } = null;
        public string? UserId { get; set; } = null;
        /// <summary>
        /// If the client is registered to use PING or PUSH modes. If is a bearer token provided by the client that will be used by the OPENID provider to authenticate the callback request to the client.
        /// </summary>
        public string? NotificationToken { get; set; } = null;
        public string? NotificationMode { get; set; } = null;
        public string? NotificationEdp { get; set; } = null;
        public int? Interval { get; set; } = null;
        public bool IsActive
        {
            get
            {
                return DateTime.UtcNow < ExpirationDateTime;
            }
        }
        public BCAuthorizeStatus LastStatus { get; set; }
        public BCAuthorizeHistory? LastHistory
        {
            get
            {
                return Histories.Where(h => h.EndDateTime == null).OrderByDescending(h => h.StartDateTime).FirstOrDefault();
            }
        }
        public DateTime ExpirationDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime? RejectionSentDateTime { get; set; }
        public DateTime? NextFetchTime { get; set; }
        public string Realm { get; set; } = null!;
        public ICollection<AuthorizationData> AuthorizationDetails
        {
            get
            {
                if (SerializedAuthorizationDetails == null) return new List<AuthorizationData>();
                return JsonSerializerExtensions.DeserializeAuthorizationDetails(SerializedAuthorizationDetails);
            }
            set
            {
                SerializedAuthorizationDetails = JsonSerializer.Serialize(value);
            }
        }
        public string? SerializedAuthorizationDetails { get; set; } = null;
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
        public ICollection<BCAuthorizeHistory> Histories { get; set; } = new List<BCAuthorizeHistory>();

        public void Confirm() => UpdateStatus(BCAuthorizeStatus.Confirmed);

        public void Pong() => UpdateStatus(BCAuthorizeStatus.Pong);

        public void Send() => UpdateStatus(BCAuthorizeStatus.Sent);

        public void Reject() => UpdateStatus(BCAuthorizeStatus.Rejected);

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
            ICollection<AuthorizationData> authorizationDetails,
            string userId,
            string notificationToken,
            string realm)
        {
            var result = new BCAuthorize
            {
                Id = GenerateId(),
                ExpirationDateTime = expirationDateTime,
                ClientId = clientId,
                Interval = interval,
                NotificationEdp = notificationEdp,
                NotificationMode = notificationMode,
                Scopes = scopes,
                UserId = userId,
                NotificationToken = notificationToken,
                Realm = realm,
                AuthorizationDetails = authorizationDetails
            };
            result.UpdateStatus(BCAuthorizeStatus.Pending);
            return result;
        }

        private void UpdateStatus(BCAuthorizeStatus newStatus, string msg = null)
        {
            if (LastHistory != null)
                LastHistory.EndDateTime = DateTime.UtcNow;
            Histories.Add(new BCAuthorizeHistory { StartDateTime = DateTime.UtcNow, Status = newStatus, Message = msg });
            LastStatus = newStatus;
            UpdateDateTime = DateTime.UtcNow;
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
