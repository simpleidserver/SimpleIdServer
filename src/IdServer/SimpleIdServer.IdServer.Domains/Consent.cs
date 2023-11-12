// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public enum ConsentStatus
    {
        PENDING = 0,
        ACCEPTED = 1
    }

    public class Consent
    {
        public Consent() { }

        public Consent(string id, string clientId, ICollection<AuthorizedScope> scopes, ICollection<string> claims, string realm)
        {
            Id = id;
            ClientId = clientId;
            Scopes = scopes;
            Claims = claims;
            Realm = realm;
            CreateDateTime = DateTime.UtcNow;
            UpdateDateTime = DateTime.UtcNow;
        }

        [JsonPropertyName(UserConsentNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(UserConsentNames.ClientId)]
        public string ClientId { get; set; } = null!;
        [JsonPropertyName(UserConsentNames.CreateDatetime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(UserConsentNames.UpdateDatetime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(UserConsentNames.Status)]
        public ConsentStatus Status { get; set; } = ConsentStatus.PENDING;
        [JsonPropertyName(GrantParameters.Scopes)]
        public ICollection<AuthorizedScope> Scopes { get; set; } = new List<AuthorizedScope>();
        [JsonPropertyName(GrantParameters.Claims)]
        public ICollection<string> Claims { get; set; } = new List<string>();
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
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public string Realm { get; set; }

        public void Merge(ICollection<string> claims, ICollection<AuthorizedScope> scopes, IEnumerable<AuthorizationData> authorizationDetails)
        {
            MergeScopes();
            MergeClaims();
            MergeAuthorizationDetails();

            void MergeScopes()
            {
                var newScopes = Scopes.Select(s => new AuthorizedScope
                {
                    AuthorizedResources = s.AuthorizedResources.Select(r => new AuthorizedResource
                    {
                        Audience = r.Audience,
                        Resource = r.Resource
                    }).ToList(),
                    Scope = s.Scope
                }).ToList();
                var unknownScopes = scopes.Where(s => !newScopes.Any(sc => sc.Scope == s.Scope));
                foreach (var unknownScope in unknownScopes)
                    newScopes.Add(unknownScope);
                foreach (var existingScope in newScopes)
                {
                    var newScope = scopes.FirstOrDefault(s => s.Scope == existingScope.Scope);
                    if (newScope == null) continue;
                    var unknownResources = newScope.AuthorizedResources.Where(r => !existingScope.AuthorizedResources.Any(ar => ar.Resource == r.Resource));
                    var resources = existingScope.AuthorizedResources;
                    foreach (var unknownResource in unknownResources)
                        resources.Add(unknownResource);
                    existingScope.AuthorizedResources = resources;
                }

                Scopes = newScopes;
            }

            void MergeClaims()
            {
                var cls = Claims.ToList();
                foreach (var cl in claims)
                {
                    if (cls.Contains(cl)) continue;
                    cls.Add(cl);
                }

                Claims = cls;
            }

            void MergeAuthorizationDetails()
            {
                var newAuthorizationDetails = AuthorizationDetails.ToList();
                var unknownAuthDataLst = authorizationDetails.Where(s => !newAuthorizationDetails.Any(sc => sc.Type == s.Type));
                foreach (var unknownAuthData in unknownAuthDataLst)
                    newAuthorizationDetails.Add(unknownAuthData);
                foreach (var existingAuthData in newAuthorizationDetails)
                {
                    var newAuthData = authorizationDetails.FirstOrDefault(s => s.Type == existingAuthData.Type);
                    if (newAuthData == null) continue;
                    if(newAuthData.Locations != null)
                    {
                        var unknownLocations = newAuthData.Locations.Where(l => !existingAuthData.Locations.Contains(l));
                        foreach (var unknownLocation in unknownLocations)
                            existingAuthData.AddLocation(unknownLocation);
                    }

                    if(newAuthData.Actions != null)
                    {
                        var unknownActions = newAuthData.Actions.Where(a => !existingAuthData.Actions.Contains(a));
                        foreach (var unknownAction in unknownActions)
                            existingAuthData.AddAction(unknownAction);
                    }

                    if(newAuthData.DataTypes != null)
                    {
                        var unknownDataTypes = newAuthData.DataTypes.Where(a => !existingAuthData.DataTypes.Contains(a));
                        foreach (var unknownDataType in unknownDataTypes)
                            existingAuthData.AddDataType(unknownDataType);
                    }

                    if(newAuthData.AdditionalData != null)
                    {
                        var unknownAddsData = newAuthData.AdditionalData.Where(t => !existingAuthData.AdditionalData.Any(kvp => kvp.Key == t.Key && kvp.Value == t.Value));
                        foreach (var unknownAddData in unknownAddsData)
                            existingAuthData.AddAdditionalData(unknownAddData.Key, unknownAddData.Value);
                    }
                }

                AuthorizationDetails = newAuthorizationDetails;
            }
        }

        public void Accept()
        {
            Status = ConsentStatus.ACCEPTED;
            UpdateDateTime = DateTime.UtcNow;
        }

        public void Update()
        {
            Status = ConsentStatus.PENDING;
            UpdateDateTime = DateTime.UtcNow;
        }
    }
}
