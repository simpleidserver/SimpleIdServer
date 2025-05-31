// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.Scim.ApiKeyAuth;

public class ApiKeysConfiguration
{
    public static List<string> AllScopes = new List<string>
    {
        "query_scim_resource", "add_scim_resource", "delete_scim_resource", "update_scim_resource", "bulk_scim_resource", "scim_provision"
    };

    public List<ApiKeyConfiguration> ApiKeys { get; set; } = new List<ApiKeyConfiguration>();

    public static ApiKeysConfiguration Default => new ApiKeysConfiguration
    {
        ApiKeys = new List<ApiKeyConfiguration>
        {
            new ApiKeyConfiguration
            {
                Owner = "IdServer",
                Value = "ba521b3b-02f7-4a37-b03c-58f713bf88e7",
                Scopes = AllScopes
            },
            new ApiKeyConfiguration
            {
                Owner = "AzureAd",
                Value = "1595a72a-2804-495d-8a8a-2c861e7a736a",
                Scopes = AllScopes
            }
        }
    };
}
