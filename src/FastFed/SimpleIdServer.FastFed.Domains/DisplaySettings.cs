// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Domains.Resources;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Domains;

public class DisplaySettings
{
    /// <summary>
    /// The name of the Provider suitable for display to end-users. 
    /// The value SHOULD be the primary textual label by which this Provider is normally displayed when presenting it to end-users. 
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = null!;
    /// <summary>
    /// A URL that points to a square image file. 
    /// Commonly used in scenarios where clicking the icon results in launching an instance of the application for the user.
    /// </summary>
    [JsonPropertyName("icon_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string IconUri { get; set; } = null;
    /// <summary>
    /// A URL that points to a logo file for the business or organization. 
    /// </summary>
    [JsonPropertyName("logo_uri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string LogoUri { get; set; } = null;
    /// <summary>
    ///  URL that points to a license granting use of the display name, icons, and logos.
    ///  o maximize interoperability, all Providers are RECOMMENDED to use an existing FastFed license such as "https://openid.net/intellectual-property/licenses/fastfed/1.0/".
    /// </summary>
    [JsonPropertyName("license")]
    public string License { get; set; } = null!;

    public List<string> Validate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(DisplayName)) result.Add(string.Format(Global.MissingParameter, "display_name"));
        if (string.IsNullOrWhiteSpace(License)) result.Add(string.Format(Global.MissingParameter, "license"));
        return result;
    }
}
