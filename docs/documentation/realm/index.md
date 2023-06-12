# Realm

A Realm is a space where you can manage Clients, Scopes, Users, Identity Providers, and Certificate Authorities. Realms are isolated from one another, but the same resource can be located in one or more Realms.

By default, there is one configured 'master' realm. It must not be removed, as doing so would render the SimpleIdServer product inoperable.

You can use the Realm to separate different environments, such as having one for the 'test' environment and another for the 'prd' environment. The Realm functions effectively with the Identity Server, but it does not work with the SCIM endpoint. 
Consequently, it is not possible to differentiate SCIM representations based on realms

You can use the Administration UI to [manage the realm](/documentation/adminui/realm.html).