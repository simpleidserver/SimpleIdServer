// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class CredentialTemplate : BaseCredentialTemplate
    {
        public  CredentialTemplate() { }

        public CredentialTemplate(BaseCredentialTemplate credentialTemplate)
        {
            TechnicalId = credentialTemplate.TechnicalId;
            Id = credentialTemplate.Id;
            Format = credentialTemplate.Format;
            CreateDateTime = credentialTemplate.CreateDateTime;
            UpdateDateTime = credentialTemplate.UpdateDateTime;
            DisplayLst = credentialTemplate.DisplayLst;
            Parameters = credentialTemplate.Parameters;
        }

        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
        [JsonIgnore]
        public ICollection<UserCredentialOffer> CredentialOffers { get; set; } = new List<UserCredentialOffer>();
        [JsonIgnore]
        public ICollection<CredentialTemplateClaimMapper> ClaimMappers { get; set; } = new List<CredentialTemplateClaimMapper>();

        public override string Serialize()
        {
            var result = JsonSerializer.Serialize(this);
            return result;
        }

        public static CredentialTemplate Deserialize(string json)
        {
            return JsonSerializer.Deserialize<CredentialTemplate>(json);
        }
    }
}
