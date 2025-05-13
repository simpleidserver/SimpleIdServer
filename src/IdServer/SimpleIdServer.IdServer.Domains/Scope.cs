// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class Scope : IEquatable<Scope>
    {
        public Scope()
        {
            ClaimMappers = new List<ScopeClaimMapper>();
        }

        public Scope(string name) : this()
        {
            Name = name;
        }

        [JsonPropertyName(ScopeNames.Source)]
        public string? Source
        {
            get; set;
        }
        [JsonPropertyName(ScopeNames.Id)]
        public string Id { get; set; }
        [JsonPropertyName(ScopeNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(ScopeNames.Type)]
        public ScopeTypes Type { get; set; } = ScopeTypes.IDENTITY;
        [JsonPropertyName(ScopeNames.Protocol)]
        public ScopeProtocols Protocol { get; set; } = ScopeProtocols.OPENID;
        [JsonPropertyName(ScopeNames.Description)]
        public string? Description { get; set; } = null;
        [JsonPropertyName(ScopeNames.IsExposedInConfigurationEdp)]
        public bool IsExposedInConfigurationEdp { get; set; }
        [JsonPropertyName(ScopeNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(ScopeNames.UpdateDatetime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(ScopeNames.Component)]
        public string? Component { set; get; } = null;
        [JsonPropertyName(ScopeNames.Action)]
        public ComponentActions? Action { get; set; }
        /// <summary>
        /// Array of strings that specifies the claims.
        /// </summary>
        [JsonPropertyName(ScopeNames.Mappers)]
        public ICollection<ScopeClaimMapper> ClaimMappers { get; set; } = new List<ScopeClaimMapper>();
        [JsonIgnore]
        public ICollection<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        [JsonIgnore]
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        [JsonIgnore]
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
        [JsonIgnore]
        public ICollection<Group> Groups { get; set; } = new List<Group>();

        public static Scope Create(string scopeName)
        {
            return new Scope
            {
                Name = scopeName,
                UpdateDateTime = DateTime.UtcNow,
                CreateDateTime = DateTime.UtcNow
            };
        }

        public bool Equals(Scope other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var target = obj as Scope;
            if (target == null)
            {
                return false;
            }

            return Equals(target);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public enum ScopeTypes
    {
        IDENTITY = 0,
        APIRESOURCE = 1,
        ROLE = 2
    }

    public enum ScopeProtocols
    {
        OPENID = 0,
        SAML = 1,
        OAUTH = 2
    }
}

public enum ComponentActions
{
    Manage = 0,
    View = 1
}