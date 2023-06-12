# Set up SCIM provisioning in Azure AD

To establish user provisioning from Azure Active Directory (AD) to the SCIM Endpoint, you can follow this tutorial [https://learn.microsoft.com/en-us/azure/active-directory/app-provisioning/use-scim-to-provision-users-and-groups#integrate-your-scim-endpoint-with-the-azure-ad-provisioning-service](https://learn.microsoft.com/en-us/azure/active-directory/app-provisioning/use-scim-to-provision-users-and-groups#integrate-your-scim-endpoint-with-the-azure-ad-provisioning-service).

When the provisioning mode is selected, the `Tenant URL` and `Secret Token` must be specified.

![Create Organization Unit](images/azuread-1.png)

Assign the value of the SCIM endpoint `http://localhost:5003` to the Tenant URL.
Set the Secret Token to one of the following values :

| Owner    | Value                                |
| -------- | ------------------------------------ |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd  | 1595a72a-2804-495d-8a8a-2c861e7a736a |