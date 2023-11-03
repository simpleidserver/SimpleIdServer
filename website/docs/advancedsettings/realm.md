# Realm

A [Realm](../glossary) is a space where you can manage Clients, Scopes, Users, External Identity Providers, and Certificate Authorities. Realms are isolated from one another, but the same resource can be located in one or more Realms.

By default, there is one configured `master` realm. It must not be removed, as doing so would render the SimpleIdServer product inoperable.

You can use the Realm to separate different environments, such as having one for the `test` environment and another for the `prd` environment. 

To add a realm, follow these steps :

1. Click `Active realm: master`.
2. Click `Add realm`.
3. Enter the details for the new Realm.
4. Click `Save`. After saving the details, the user-agent will be redirected to the new realm.

You can switch the active realm by clicking on `Active realm: active realm`.

## Disable Realm

By default, SimpleIdServer is configured to use the Realm. If you do not want to use it, you can disable it by updating the `appsettings.json` configuration files.

To disable the Realm, follow these steps:

1. Open the [IdentityServer](../installation#create-identityserver-project) project and edit the `appsettings.json` file.
2. Set the `IsRealmEnabled` property to `false` and save the file.
3. Open the [IdentityServer website](../installation#create-identityserver-website-project) and edit the `appsettings.json` file.
3. Set the `IsRealmEnabled` property to `false`, remove the master realm from the `DefaultSecurityOptions.Issuer` property, and save the file.