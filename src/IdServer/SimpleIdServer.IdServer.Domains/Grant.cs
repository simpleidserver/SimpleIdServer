// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json;
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
        public string UserId { get; set; } = null!;
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
        /// <summary>
        /// JSON Object as defined in [I-D.ietf-oauth-rar] containing all authorization details as requested and consented in one or more authorization requests associated with the respective grant.
        /// </summary>
        [JsonPropertyName(GrantParameters.AuthorizationDetails)]
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
        [JsonIgnore]
        public string? SerializedAuthorizationDetails { get; set; } = null;

        public void Merge(ICollection<string> claims, ICollection<AuthorizedScope> scopes, IEnumerable<AuthorizationData> authorizationDetails)
        {
            MergeScopes();
            MergeClaims();
            MergeAuthorizationDetails();

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

            void MergeAuthorizationDetails()
            {
                var unknownAuthDataLst = authorizationDetails.Where(s => !AuthorizationDetails.Any(sc => sc.Type == s.Type));
                foreach (var unknownAuthData in unknownAuthDataLst)
                    AuthorizationDetails.Add(unknownAuthData);
                foreach (var existingAuthData in AuthorizationDetails)
                {
                    var newAuthData = authorizationDetails.FirstOrDefault(s => s.Type == existingAuthData.Type);
                    if (newAuthData == null) continue;
                    var unknownLocations = newAuthData.Locations.Where(l => !existingAuthData.Locations.Contains(l));
                    foreach (var unknownLocation in unknownLocations)
                        existingAuthData.Locations.Add(unknownLocation);
                    var unknownActions = newAuthData.Actions.Where(a => !existingAuthData.Actions.Contains(a));
                    foreach (var unknownAction in unknownActions)
                        existingAuthData.Actions.Add(unknownAction);
                    var unknownDataTypes = newAuthData.DataTypes.Where(a => !existingAuthData.DataTypes.Contains(a));
                    foreach (var unknownDataType in unknownDataTypes)
                        existingAuthData.DataTypes.Add(unknownDataType);
                }

                AuthorizationDetails = AuthorizationDetails;
            }
        }

        public void Revoke()
        {
            UpdateDateTime = DateTime.UtcNow;
            Status = GrantTypeStatus.REVOKED;
        }

        public static Grant Create(string clientId, string userId, ICollection<string> claims, ICollection<AuthorizedScope> scopes, ICollection<AuthorizationData> authorizationDataLst)
        {
            return new Grant
            {
                Id = Guid.NewGuid().ToString(),
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Claims = claims,
                Scopes = scopes,
                Status = GrantTypeStatus.ACTIVE,
                ClientId = clientId,
                UserId = userId,
                AuthorizationDetails = authorizationDataLst
            };
        }
    }
}
