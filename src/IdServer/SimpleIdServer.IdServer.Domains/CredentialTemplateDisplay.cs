// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class CredentialTemplateDisplay
    {
        [JsonIgnore]
        public string Id { get; set; } = null!;
        /// <summary>
        /// Display name for the credential.
        /// </summary>
        [JsonPropertyName(CredentialTemplateDisplayNames.Name)]
        public string Name { get; set; } = null!;
        /// <summary>
        /// Identifies the language of this object.
        /// </summary>
        [JsonPropertyName(CredentialTemplateDisplayNames.Locale)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; } = null;
        /// <summary>
        /// URL where the wallet can obtain a logo of the credential from the credential issuer.
        /// </summary>
        [JsonIgnore]
        public string? LogoUrl { get; set; } = null;
        /// <summary>
        /// Alternative text of a logo image.
        /// </summary>
        [JsonIgnore]
        public string? LogoAltText { get; set; } = null;
        /// <summary>
        /// Logo of the credential.
        /// </summary>
        [JsonPropertyName(CredentialTemplateDisplayNames.Logo)]
        public CredentialTemplateDisplayLogo? Logo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LogoUrl) && string.IsNullOrWhiteSpace(LogoAltText)) return null;
                return new CredentialTemplateDisplayLogo
                {
                    Logo = LogoUrl,
                    AltText = LogoAltText
                };
            }
        }
        /// <summary>
        /// Description of a credential.
        /// </summary>
        [JsonPropertyName(CredentialTemplateDisplayNames.Description)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; } = null;
        /// <summary>
        /// Background color of the Credential represented as numerical color values defined in CSS Color Module
        /// </summary>
        [JsonPropertyName(CredentialTemplateDisplayNames.BackgroundColor)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BackgroundColor { get; set; } = null;
        /// <summary>
        /// Text color of the Credential represented as numerical color values defined in CSS Color
        /// </summary>
        [JsonPropertyName(CredentialTemplateDisplayNames.TextColor)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TextColor { get; set; } = null;
        [JsonIgnore]
        public string CredentialTemplateId { get; set; } = null!;
        [JsonIgnore]
        public CredentialTemplate CredentialTemplate { get; set; } = null!;
    }

    public record CredentialTemplateDisplayLogo
    {
        [JsonPropertyName(CredentialTemplateDisplayNames.Url)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Logo { get; set; } = null;
        [JsonPropertyName(CredentialTemplateDisplayNames.AltText)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AltText { get; set; } = null;
    }
}
