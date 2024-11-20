# Realm

A [Realm](../glossary) is a space where you can manage Clients, Scopes, Users, External Identity Providers, and Certificate Authorities. Realms are isolated from one another, but the same resource can be located in one or more Realms.

By default, there is one configured `master` realm. It must not be removed, as doing so would render the SimpleIdServer product inoperable.

You can use the Realm to separate different environments, such as having one for the `test` environment and another for the `prd` environment. 

To add a realm, follow these steps :

1. Click `Active realm: master`.
2. Click `Add realm`.
3. Enter the details for the new Realm.
4. Click `Save`. 
5. Click `Choose realm`, select the new realm, and click the `Select` button.
6. You'll be redirected to the authentication page. Submit the `administrator` credentials to access the realm.

By default, the `administrator` user has access to all the `realm`.

## Permissions

It is possible to assign user permissions to a realm and to limit access accordingly.
For each component present in the UI, there are two scopes defined for the `manage` and `view` permissions. 
For example, for the `Clients` component, the master realm defines two scopes: `master/clients/view` and `master/clients/manage`. 
These scopes can be assigned to one or more groups, and the groups can be assigned to one or more users.

For example, to grant `manage` access to the `Clients` component of the `master` realm, execute the following steps:

1. Create a new group named `ClientMaster`.
2. Navigate to the new group and select the `Role` tab.
3. Select the `<realm>/clients/manage` scope and click on the `Save` button.
4. Navigate to a user and select the `Groups` tab.
5. Select the `ClientMaster` group and click on the `Save` button.

The user is now configured to manage the `clients` present in the realm.

## Disable Realm

By default, SimpleIdServer is configured to use the Realm. If you do not want to use it, you can disable it by updating the `appsettings.json` configuration files.

To disable the Realm, follow these steps:

1. Open the [IdentityServer](../installation/dotnettemplate#create-identityserver-project) project and edit the `appsettings.json` file.
2. Set the `IsRealmEnabled` property to `false` and save the file.
3. Open the [IdentityServer website](../installation/dotnettemplate#create-identityserver-website-project) and edit the `appsettings.json` file.