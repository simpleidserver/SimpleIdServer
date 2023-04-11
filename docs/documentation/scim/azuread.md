# Set up SCIM provisioning in Azure AD

> [!NOTE]
> By default, SCIM is configured to use **API KEY authentication**. Any clients who have those keys can execute SCIM operations.

To Integrate the SCIM endpoint with the Azure AD Provisioning Service you can follow this tutorial [https://learn.microsoft.com/en-us/azure/active-directory/app-provisioning/use-scim-to-provision-users-and-groups#integrate-your-scim-endpoint-with-the-azure-ad-provisioning-service](https://learn.microsoft.com/en-us/azure/active-directory/app-provisioning/use-scim-to-provision-users-and-groups#integrate-your-scim-endpoint-with-the-azure-ad-provisioning-service).

When the provisioning mode must be selected, the `Tenant URL` and `Secret Token` must be specified.

![Create Organization Unit](images/azuread-1.png)

Tenant URL must be the URL of your SCIM endpoint for example `http://localhost:5003`.

The Secret Token must be one of the token defined in the `appsettings.json` file of the SCIM project.

By default, there are two tokens :

| Owner    | Value                                |
| -------- | ------------------------------------ |
| IdServer | ba521b3b-02f7-4a37-b03c-58f713bf88e7 |
| AzureAd  | 1595a72a-2804-495d-8a8a-2c861e7a736a |