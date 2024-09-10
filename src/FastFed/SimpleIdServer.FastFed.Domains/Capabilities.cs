// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.FastFed.Domains.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Domains;

public class Capabilities
{
    /// <summary>
    /// A list of URNs specifying the single sign-in authentication protocols supported by the Provider.
    /// For example, the value "urn:ietf:params:fastfed:1.0:authentication:saml:2.0:enterprise" indicates the provider implements the FastFed Enterprise SAML Profile.
    /// If no value is specified by the Application Provider for authentication_profiles, this indicates the Provider does not require authentication.
    /// </summary>
    [JsonPropertyName("authentication_profiles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> AuthenticationProfiles { get; set; } = null;
    /// <summary>
    ///  A list of URNs specifying the user provisioning capabilities supported by the Provider.
    ///  For example, the value "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise" indicates a Provider supports full user provisioning (and deprovisioning) as defined in the FastFed Profile For SCIM Provisioning.
    /// </summary>
    [JsonPropertyName("provisioning_profiles")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> ProvisioningProfiles { get; set; } = null;
    /// <summary>
    /// A list of URNs specifying the schemas grammars understood by the Provider.
    /// RECOMMENDS the use of SCIM.
    /// </summary>
    [JsonPropertyName("schema_grammars")]
    public List<string> SchemaGrammars { get; set; } = new List<string>();
    /// <summary>
    /// A list of JWA signing algorithms supported by the Provider. 
    /// Used for signing request objects within the FastFed Handshake.
    /// </summary>
    [JsonPropertyName("signing_algorithms")]
    public List<string> SigningAlgorithms { get; set; } = new List<string>();

    public List<string> Validate()
    {
        var result = new List<string>();
        if (SigningAlgorithms == null || !SigningAlgorithms.Any()) result.Add(string.Format(Global.MissingParameter, "signing_algorithms"));
        if (SchemaGrammars == null || !SchemaGrammars.Any()) result.Add(string.Format(Global.MissingParameter, "schema_grammars"));
        return result;
    }
}