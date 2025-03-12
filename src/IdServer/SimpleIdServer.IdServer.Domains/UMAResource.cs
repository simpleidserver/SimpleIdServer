// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using SimpleIdServer.IdServer.Helpers.Models;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    [JsonConverter(typeof(TranslatableConverter<UMAResource>))]
    public class UMAResource : ITranslatable
    {
        public UMAResource() { }

        public UMAResource(string id, DateTime createDateTime, string realm)
        {
            Id = id;
            CreateDateTime = createDateTime;
            UpdateDateTime = createDateTime;
            Realm = realm;
            Scopes = new List<string>();
        }

        [JsonPropertyName(UMAResourceNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(UMAResourceNames.IconUri)]
        public string? IconUri { get; set; } = null;
        [JsonPropertyName(UMAResourceNames.Type)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; set; } = null;
        [JsonIgnore]
        public string? Subject { get; set; } = null;
        [JsonPropertyName(UMAResourceNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(UMAResourceNames.ResourceScopes)]
        public ICollection<string> Scopes { get; set; } = new List<string>();
        [JsonIgnore]
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();
        [JsonIgnore]
        public ICollection<UMAResourcePermission> Permissions { get; set; } = new List<UMAResourcePermission>();
        [JsonIgnore]
        public string? Description
        {
            get
            {
                return Translate("description");
            }
        }
        [JsonIgnore]
        public string? Name
        {
            get
            {
                return Translate("name");
            }
        }
        public string Realm { get; set; } = null!;

        public void UpdateTranslations(IEnumerable<Translation> translations)
        {
            Translations.Clear();
            Translations = translations.Where(t => t.Key == "description" || t.Key == "name").ToList();
        }

        public void ReplacePermissions(ICollection<UMAResourcePermission> permissions)
        {
            Permissions.Clear();
            foreach (var permission in permissions)
            {
                Permissions.Add(permission);
            }
        }

        public bool Equals(UMAResource other)
        {
            if (other == null)
                return false;

            return this.GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode() => Id.GetHashCode();

        private string? Translate(string key)
        {
            var translation = Translations.FirstOrDefault(t => t.Key == key && t.Language == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
            return translation?.Value;
        }
    }
}
