// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class AppProviderProvisioningProfileMappingsResult
{
    /// <summary>
    ///  Boolean value indicating whether the Application Provider supports provisioning of nested Group memberships.
    /// </summary>
    [JsonPropertyName("can_support_nested_groups")]
    public bool CanSupportedNestedGroups { get; set; }
    /// <summary>
    /// Number indicating the maximum number of Group membership changes that can be included in a single PATCH request.
    /// </summary>
    [JsonPropertyName("max_group_membership_changes")]
    public int MaxGroupMembershipChanges { get; set; }
    /// <summary>
    /// A structure specifying the attributes to be provisioned.
    /// </summary>
    [JsonPropertyName("desired_attributes")]
    public ScimDesiredAttributes DesiredAttributes { get; set; }
}

public class ScimDesiredAttributes
{
    [JsonPropertyName(Constants.SchemaGrammarName)]
    public SchemaGrammarDesiredAttributes DesiredAttributes { get; set; }
}

public class SchemaGrammarDesiredAttributes
{
    [JsonPropertyName("required_user_attributes")]
    public List<string> RequiredUserAttributes { get; set; }
    [JsonPropertyName("optional_user_attributes")]
    public List<string> OptionalUserAttributes { get; set; }
    [JsonPropertyName("required_group_attributes")]
    public List<string> RequiredGroupAttributes { get; set; }
    [JsonPropertyName("optional_group_attributes")]
    public List<string> OptionalGroupAttributes { get; set; }
}