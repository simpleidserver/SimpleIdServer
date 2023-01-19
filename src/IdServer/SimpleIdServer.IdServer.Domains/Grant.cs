// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public enum GrantTypeStatus
    {
        ACTIVE = 0,
        REVOKED = 1
    }

    public class Grant
    {
        [JsonIgnore]
        public string Id { get; set; } = null!;
        [JsonIgnore]
        public string ClientId { get; set; } = null!;
        [JsonIgnore]
        public DateTime CreateDateTime { get; set; }
        [JsonIgnore]
        public DateTime UpdateDateTime { get; set; }
        [JsonIgnore]
        public GrantTypeStatus Status { get; set; }
        [JsonPropertyName(GrantParameters.Claims)]
        /// <summary>
        /// JSON array containing the names of all OpenID Connect claims.
        /// </summary>
        public ICollection<string> Claims { get; set; } = new List<string>();
        [JsonPropertyName(GrantParameters.Scopes)]
        /// <summary>
        ///  JSON array where every entry contains a scope field and may contain one or more resource fields.
        /// </summary>
        public ICollection<AuthorizedScope> Scopes { get; set; } = new List<AuthorizedScope>();

        public void Merge(ICollection<string> claims, ICollection<AuthorizedScope> scopes)
        {
            MergeScopes();
            MergeClaims();

            void MergeScopes()
            {
                var unknownScopes = scopes.Where(s => !Scopes.Any(sc => sc.Scope == s.Scope));
                foreach (var unknownScope in unknownScopes)
                    Scopes.Add(unknownScope);
                foreach (var existingScope in Scopes)
                {
                    var newScope = scopes.FirstOrDefault(s => s.Scope == existingScope.Scope);
                    if (newScope == null) continue;
                    var unknownResources = newScope.Resources.Where(r => !existingScope.Resources.Contains(r));
                    var resources = existingScope.Resources;
                    foreach (var unknownResource in unknownResources)
                        resources.Add(unknownResource);
                    existingScope.Resources = resources;
                }
            }

            void MergeClaims()
            {
                var cls = Claims;
                foreach(var cl in claims)
                {
                    if (cls.Contains(cl)) continue;
                    cls.Add(cl);
                }

                Claims = cls;
            }
        }

        public void Revoke()
        {
            UpdateDateTime = DateTime.UtcNow;
            Status = GrantTypeStatus.REVOKED;
        }

        public static Grant Create(string clientId, ICollection<string> claims, ICollection<AuthorizedScope> scopes)
        {
            return new Grant
            {
                Id = Guid.NewGuid().ToString(),
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Claims = claims,
                Scopes = scopes,
                Status = GrantTypeStatus.ACTIVE,
                ClientId = clientId
            };
        }
    }
}
