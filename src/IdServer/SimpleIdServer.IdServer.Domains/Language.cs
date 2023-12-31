// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class Language
{
    public const string Default = "en";

    private const string TranslationKeyName = "language_description_{0}";

    [JsonPropertyName(LanguageNames.Code)]
    public string Code { get; set; } = null!;

    [JsonPropertyName(LanguageNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }

    [JsonPropertyName(LanguageNames.UpdateDateTime)]
    public DateTime UpdateDateTime { get; set; }

    [JsonIgnore]
    public string TranslationKey
    {
        get
        {
            return string.Format(TranslationKeyName, Code);
        }
    }

    [JsonPropertyName(LanguageNames.Description)]
    public string? Description { get; set; } = null;

    [JsonIgnore]
    public ICollection<Translation> Descriptions { get; set; } = new List<Translation>();
}
