// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class CredentialTemplate
    {
        [JsonIgnore]
        public string TechnicalId { get; set; } = null!;
        /// <summary>
        /// Identifying the respective object.
        /// </summary>
        [JsonPropertyName(CredentialTemplateNames.Id)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; } = null;
        /// <summary>
        /// Format of the credential, e.g. jwt_vc_json or ldp_vc.
        /// </summary>
        public string Format { get; set; } = null!;
        [JsonIgnore]
        public DateTime CreateDateTime { get; set; }
        [JsonIgnore]
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Array of objects, where each object contains the display properties of the supported credential for a certain language.
        /// </summary>
        [JsonPropertyName(CredentialTemplateNames.Display)]
        public ICollection<CredentialTemplateDisplay> DisplayLst { get; set; } = new List<CredentialTemplateDisplay>();
        [JsonIgnore]
        public ICollection<CredentialTemplateParameter> Parameters { get; set; } = new List<CredentialTemplateParameter>();
        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    }
}
