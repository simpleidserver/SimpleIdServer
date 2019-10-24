// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResource : ICloneable, IEquatable<UMAResource>
    {
        public UMAResource(string id, DateTime createDateTime)
        {
            Id = id;
            CreateDateTime = createDateTime;
            Scopes = new List<string>();
            Descriptions = new List<OAuthTranslation>();
            Names = new List<OAuthTranslation>();
            Permissions = new List<UMAResourcePermission>();
        }

        public string Id { get; set; }
        public ICollection<string> Scopes { get; set; }
        public ICollection<OAuthTranslation> Descriptions { get; set; }
        public string IconUri { get; set; }
        public ICollection<OAuthTranslation> Names { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public ICollection<UMAResourcePermission> Permissions { get; set; }
        public DateTime CreateDateTime { get; set; }

        public void AddDescription(string language, string value)
        {
            Descriptions.Add(new OAuthTranslation($"{Id}_uma_resource", value, language));
        }

        public void AddName(string language, string value)
        {
            Names.Add(new OAuthTranslation($"{Id}_uma_resource", value, language));
        }

        public object Clone()
        {
            return new UMAResource(Id, CreateDateTime)
            {
                Scopes = Scopes.ToList(),
                Descriptions = Descriptions.Select(d => (OAuthTranslation)d.Clone()).ToList(),
                IconUri = IconUri,
                Names = Names.Select(d => (OAuthTranslation)d.Clone()).ToList(),
                Type = Type,
                Subject = Subject,
                Permissions = Permissions.Select(p => (UMAResourcePermission)p.Clone()).ToList()
            };
        }

        public bool Equals(UMAResource other)
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