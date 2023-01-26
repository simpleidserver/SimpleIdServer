// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.UMAResources
{
    public class UMAResourceRequest
    {
        [JsonPropertyName(UMAResourceNames.ResourceScopes)]
        [BindProperty(Name = UMAResourceNames.ResourceScopes)]
        public ICollection<string> Scopes { get; set; } = new List<string>();
        [JsonPropertyName(UMAResourceNames.IconUri)]
        [BindProperty(Name = UMAResourceNames.IconUri)]
        public string? IconUri { get; set; } = null;
        [JsonPropertyName(UMAResourceNames.Type)]
        [BindProperty(Name = UMAResourceNames.Type)]
        public string? Type { get; set; } = null;
        [JsonPropertyName(UMAResourceNames.Subject)]
        [BindProperty(Name = UMAResourceNames.Subject)]
        public string? Subject { get; set; } = null;
        [JsonIgnore]
        public ICollection<UMAResourceTranslation> Translations = new List<UMAResourceTranslation>();
    }

    public class UMAResourceTranslation
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
