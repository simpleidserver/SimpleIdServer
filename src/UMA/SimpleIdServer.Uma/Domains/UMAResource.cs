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
            Permissions = new List<UMAResourcePermission>();
            Translations = new List<UMAResourceTranslation>();
        }

        public string Id { get; set; }
        public ICollection<string> Scopes { get; set; }
        public string IconUri { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ICollection<OAuthTranslation> Descriptions
        {
            get
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "description").ToList();
            }
        }
        public ICollection<OAuthTranslation> Names
        {
            get
            {
                return Translations.Select(t => t.Translation).Where(t => t.Type == "name").ToList();
            }
        }
        public ICollection<UMAResourceTranslation> Translations { get; set; }
        public ICollection<UMAResourcePermission> Permissions { get; set; }

        public void ClearDescriptions()
        {
            ClearTranslations("description");
        }

        public void ClearNames()
        {
            ClearTranslations("name");
        }

        public void AddDescription(string language, string value)
        {
            AddDescription(new OAuthTranslation($"{Id}_uma_resource", value, language));
        }

        public void AddDescription(OAuthTranslation translation)
        {
            translation.Type = "description";
            Translations.Add(new UMAResourceTranslation
            {
                Translation = translation
            });
        }

        public void AddName(string language, string value)
        {
            AddName(new OAuthTranslation($"{Id}_uma_resource", value, language));
        }

        public void AddName(OAuthTranslation translation)
        {
            translation.Type = "name";
            Translations.Add(new UMAResourceTranslation
            {
                Translation = translation
            });
        }

        public void ReplacePermissions(ICollection<UMAResourcePermission> permissions)
        {
            Permissions.Clear();
            foreach(var permission in permissions)
            {
                Permissions.Add(permission);
            }
        }

        public object Clone()
        {
            return new UMAResource(Id, CreateDateTime)
            {
                Scopes = Scopes.ToList(),
                IconUri = IconUri,
                Translations = Translations.Select(t => (UMAResourceTranslation)t.Clone()).ToList(),
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

        private void ClearTranslations(string type)
        {
            int length = Translations.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                var translation = Translations.ElementAt(i);
                if (translation.Translation.Type == type)
                {
                    Translations.Remove(translation);
                }
            }
        }
    }
}