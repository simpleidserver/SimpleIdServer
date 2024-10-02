// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Provisioning.Scim;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Authentication.Saml;

public class SamlEntrepriseMappingsResult
{
    /// <summary>
    /// A structure describing the attribute to use as the SAML Subject.
    /// </summary>
    [JsonPropertyName("saml_subject")]
    public SamlSubject SamlSubject { get; set; }
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("desired_attributes")]
    public DesiredAttributes DesiredAttributes { get; set; }
}

public class SamlSubject
{
    [JsonPropertyName(SimpleIdServer.FastFed.Provisioning.Scim.Constants.SchemaGrammarName)]
    public string Username { get; set; }
}