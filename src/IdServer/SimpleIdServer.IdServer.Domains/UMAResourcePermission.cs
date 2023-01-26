// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class UMAResourcePermission
    {
        public UMAResourcePermission() { }

        public UMAResourcePermission(string id, DateTime createDateTime)
        {
            Id = id;
            CreateDateTime = createDateTime;
        }

        public UMAResourcePermission(string id, DateTime createDatTime, ICollection<string> scopes) : this(id, createDatTime)
        {
            Scopes = scopes;
        }

        [JsonIgnore]
        public string Id { get; set; } = null!;
        [JsonIgnore]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(UMAResourcePermissionNames.Scopes)]
        public ICollection<string> Scopes { get; set; } = new List<string>();
        [JsonPropertyName(UMAResourcePermissionNames.Claims)]
        public ICollection<UMAResourcePermissionClaim> Claims { get; set; } = new List<UMAResourcePermissionClaim>();

        public bool Equals(UMAResourcePermission other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
