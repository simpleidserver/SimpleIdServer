# Identity Management

The System for Cross-domain Identity Management (SCIM) protocol version 2.0 is utilized for managing the Identity.

By default, the API is protected by an API Key. 
Any client wishing to execute an operation must include one of the following keys in the `Authorization` header.

| Owner    | Value                                |
| -------- | ------------------------------------ |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd  | 1595a72a-2804-495d-8a8a-2c861e7a736a |

The keys have access to the following scopes

| Scope                | Description                       |
| -------------------- | --------------------------------- |
| query_scim_resource  | Retrieve resource attributes      |
| add_scim_resource    | Add resource                      |
| delete_scim_resource | Delete resource                   |
| update_scim_resource | Update resource                   |
| bulk_scim_resource   | Allows to execute bulk operations |

To automatically add users from SCIM to the Identity Server, an Identity Provisioning process must be configured.
For more information, [read the documentation](/documentation/identityprovisioning/scim.md).