// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace SimpleIdServer.Vc.Models
{
    [JsonConverter(typeof(BaseCredentialTemplateJsonConverter))]
    public class BaseCredentialTemplate
    {
        [JsonPropertyName(CredentialTemplateNames.TechnicalId)]
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
        [JsonPropertyName(CredentialTemplateNames.Format)]
        public string Format { get; set; } = null!;
        [JsonPropertyName(CredentialTemplateNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(CredentialTemplateNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        /// <summary>
        /// Array of objects, where each object contains the display properties of the supported credential for a certain language.
        /// </summary>
        [JsonPropertyName(CredentialTemplateNames.Display)]
        public ICollection<CredentialTemplateDisplay> DisplayLst { get; set; } = new List<CredentialTemplateDisplay>();
        [JsonPropertyName(CredentialTemplateNames.Parameters)]
        public ICollection<CredentialTemplateParameter> Parameters { get; set; } = new List<CredentialTemplateParameter>();
        [JsonIgnore]
        public CredentialTemplateDisplay Display
        {
            get
            {
                var result = DisplayLst.FirstOrDefault(d => d.Locale == Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
                return result ?? DisplayLst.FirstOrDefault();
            }
        }

        public virtual string Serialize() => JsonSerializer.Serialize(this);
    }
}
