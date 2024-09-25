// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Domains.Resources;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Domains;

public class ProviderContactInformation
{
    /// <summary>
    /// The legal name of the business or organization which operates the Provider.
    /// </summary>
    [JsonPropertyName("organization")]
    public string Organization { get; set; } = null!;
    /// <summary>
    /// A phone number to contact the Provider. 
    /// </summary>
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = null!;
    /// <summary>
    /// An email address to contact the Provider. 
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    public List<string> Validate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(Organization)) result.Add(string.Format(Global.MissingParameter, "organization"));
        if (string.IsNullOrWhiteSpace(Phone)) result.Add(string.Format(Global.MissingParameter, "phone"));
        if (string.IsNullOrWhiteSpace(Email)) result.Add(string.Format(Global.MissingParameter, "email"));
        return result;
    }
}
