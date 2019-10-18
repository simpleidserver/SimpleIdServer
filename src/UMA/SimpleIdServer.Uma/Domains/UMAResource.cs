using SimpleIdServer.OAuth.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Uma.Domains
{
    public class UMAResource : ICloneable
    {
        public UMAResource(string id)
        {
            Id = id;
            Scopes = new List<string>();
            Descriptions = new List<OAuthTranslation>();
            Names = new List<OAuthTranslation>();
        }

        public string Id { get; set; }
        public ICollection<string> Scopes { get; set; }
        public ICollection<OAuthTranslation> Descriptions { get; set; }
        public string IconUri { get; set; }
        public ICollection<OAuthTranslation> Names { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }

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
            return new UMAResource(Id)
            {
                Scopes = Scopes.ToList(),
                Descriptions = Descriptions.Select(d => (OAuthTranslation)d.Clone()).ToList(),
                IconUri = IconUri,
                Names = Names.Select(d => (OAuthTranslation)d.Clone()).ToList(),
                Type = Type,
                Subject = Subject
            };
        }
    }
}