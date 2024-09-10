// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Models;

public abstract class BaseProviderMetadata
{
    /// <summary>
    /// A URI which serves as a globally unique name for a participant in a FastFed exchange. 
    /// </summary>
    [JsonPropertyName("entity_id")]
    public string EntityId { get; set; } = null!;
    /// <summary>
    /// A Domain Name or the Provider. Serves as a unique key for programmatic use cases that require awareness of a distinct Provider identity. 
    /// </summary>
    [JsonPropertyName("provider_domain")]
    public string ProviderDomain { get; set; } = null!;
    /// <summary>
    /// A structure describing the contact information for the Provider.
    /// </summary>
    [JsonPropertyName("provider_contact_information")]
    public ProviderContactInformation ContactInformation { get; set; } = new ProviderContactInformation();
    [JsonPropertyName("display_settings")]
    public DisplaySettings DisplaySettings { get; set; } = new DisplaySettings();
    /// <summary>
    /// Capabilities describe the supported behaviors of a Provider.
    /// </summary>
    [JsonPropertyName("capabilities")]
    public Capabilities Capabilities { get; set; } = new Capabilities();

    public List<string> Validate()
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(EntityId)) result.Add(string.Empty);
        if (string.IsNullOrWhiteSpace(ProviderDomain)) result.Add(string.Empty);
        if (ContactInformation == null) result.Add(string.Empty);
        else result.AddRange(ContactInformation.Validate());
        if (DisplaySettings == null) result.Add(string.Empty);
        else result.AddRange(DisplaySettings.Validate());
        if (Capabilities == null) result.Add(string.Empty);
        else result.AddRange(Capabilities.Validate());
        return result;
    }

    public List<string> CheckCompatibility(Capabilities cap)
    {
        var result = new List<string>();
        if(cap.AuthenticationProfiles != null && Capabilities.AuthenticationProfiles != null)
            result.AddRange(Capabilities.AuthenticationProfiles.Except(cap.AuthenticationProfiles).Select(c => string.Empty).ToList());
        if(cap.ProvisioningProfiles != null && Capabilities.ProvisioningProfiles != null)
            result.AddRange(Capabilities.ProvisioningProfiles.Except(cap.ProvisioningProfiles).Select(c => string.Empty).ToList());
        if (cap.SchemaGrammars != null && Capabilities.SchemaGrammars != null)
            result.AddRange(Capabilities.SchemaGrammars.Except(cap.SchemaGrammars).Select(c => string.Empty).ToList());
        if (cap.SigningAlgorithms != null && Capabilities.SigningAlgorithms != null)
            result.AddRange(Capabilities.SigningAlgorithms.Except(cap.SigningAlgorithms).Select(c => string.Empty).ToList());
        return result;
    }

    protected abstract List<string> InternalValidate();
}
