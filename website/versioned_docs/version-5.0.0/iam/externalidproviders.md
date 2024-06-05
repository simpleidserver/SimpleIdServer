# External identity providers

SimpleIdServer can utilize external [Identity Providers](../glossary) to authenticate the end-user.

When the end-user authenticates for the first time with their external account, a local account will be automatically created in your [Identity Provider](../glossary).

This process is also known as the [Just-In-Time Provisioning](../glossary) workflow.

Currently, SimpleIdServer only supports [Facebook](#facebook) authentication.

## Facebook

To use Facebook as an external Identity Provider, follow these steps :

1. Navigate to the  `Authentication` menu and click on the `Authentication` link.
2. Select the `External identity providers` tab and click on the button `Add Identity Provider` button.
3. Under `Identity Provider Type`, choose `Facebook` and click on the `Next` button.
4. Fill-in the form like this and click on the `Next` button. 

| Key          | Value         |
| ------------ | ------------- |
| Name         | OtherFacebook |
| Display Name | New facebook  |
| Description  | Facebook      |

5. Follow this [tutorial](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-7.0) to obtain the `AppId` and `AppSecret`. Then, complete the form accordingly  and click `Add`.

## Negotiate

To enable Windows Authentication (also known as Negotiate, Kerberos, or NTLM authentication), follow these steps:

1. Navigate o the `Authentication` menu and click on the `Authentication` link.
2. Select the `External identity providers` tab and click on the button `Add Identity Provider` button.
3. Under `Identity Provider Type`, choose `Facebook` and click on the `Next` button.
4. Fill-in the form like this and click on the `Next` button. 

| Key          | Value         |
| ------------ | ------------- |
| Name         | Windows       |
| Display Name | Windows       |
| Description  | Windows       |

5. Click on the `Add` button to confirm the creation and navigate to the new mapping rule.
6. Click on the `Mappers` tab, select and remove all the rows.
7. Click on the `Add mapper` button and add two rules with the following content:

| Key               | Value                                                      |
| ----------------- | ---------------------------------------------------------- |
| Mapper Type       | Subject                                                    |
| Name              | Subject                                                    |
| Source Claim Name | http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name |

| Key               | Value                                                              |
| ----------------- | ------------------------------------------------------------------ |
| Mapper Type       | Identifier                                                         |
| Name              | Identifier                                                         |
| Source Claim Name | http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid |

## Mapping

The [Just-In-Time Provisioning](../glossary) workflow utilizes a list of mapping rules to transform incoming Claims into a local account.

There are two types of Mapping Rules:

* [User attribute](../glossary) : It is a dynamic user claim, for example, DateOfBirth.
* [User property](../glossary) : It is a static user claim, for example, FirstName or Email.