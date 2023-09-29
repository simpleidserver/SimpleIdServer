# OPENID

The SimpleIdServer's [Identity Provider](../glossary) can acts as an OPENID server.

## Supported Grant Types

The following Grant Types are supported

| Name                                                 | RFC  |
| ---------------------------------------------------- | ---- |
| authorization_code                                   | [https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.1](https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.1)     |
| client_credentials                                   | [https://datatracker.ietf.org/doc/html/rfc6749#section-4.4](https://datatracker.ietf.org/doc/html/rfc6749#section-4.4)     |
| password                                             | [https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.3](https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.3)     |
| refresh_token                                        | [https://datatracker.ietf.org/doc/html/rfc6749#section-1.5](https://datatracker.ietf.org/doc/html/rfc6749#section-1.5)     |
| urn:openid:params:grant-type:ciba                    | [https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html](https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html)     |
| urn:ietf:params:oauth:grant-type:device_code         | [https://www.ietf.org/rfc/rfc8628.html#section-3.4](https://www.ietf.org/rfc/rfc8628.html#section-3.4)     |
| urn:ietf:params:oauth:grant-type:pre-authorized_code | [https://openid.net/specs/openid-4-verifiable-credential-issuance-1_0-10.html](https://openid.net/specs/openid-4-verifiable-credential-issuance-1_0-10.html)     |
| urn:ietf:params:oauth:grant-type:uma-ticket          | [https://docs.kantarainitiative.org/uma/wg/rec-oauth-uma-grant-2.0.html](https://docs.kantarainitiative.org/uma/wg/rec-oauth-uma-grant-2.0.html)     |

## Supported authentication methods

The following client authentication methods are supported

| Name                        | RFC |
| --------------------------- | --- |
| client_secret_basic         | [https://www.rfc-editor.org/rfc/rfc7591.html#section-2.3.1](https://www.rfc-editor.org/rfc/rfc7591.html#section-2.3.1) |
| client_secret_jwt           | [https://www.rfc-editor.org/rfc/rfc7523](https://www.rfc-editor.org/rfc/rfc7523)
| client_secret_post          | [https://www.rfc-editor.org/rfc/rfc7591.html#section-2.3.1](https://www.rfc-editor.org/rfc/rfc7591.html#section-2.3.1) |
| private_key_jwt             | [https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication](https://openid.net/specs/openid-connect-core-1_0.html#ClientAuthentication) |
| self_signed_tls_client_auth | [https://www.rfc-editor.org/rfc/rfc8705.html](https://www.rfc-editor.org/rfc/rfc8705.html) |
| tls_client_auth             | [https://www.rfc-editor.org/rfc/rfc8705.html](https://www.rfc-editor.org/rfc/rfc8705.html) |
| pkce                        | [https://datatracker.ietf.org/doc/html/rfc7636](https://datatracker.ietf.org/doc/html/rfc7636) |

## Clients

The [Client](../glossary) is the application that is attempting to act on the user's behalf or access to user's resource.

In SimpleIdServer, clients are grouped by [Categories](../glossary) and each category belongs to a [Template](../glossary).

There are three templates

<table>
    <thead>
        <th>Template</th>
        <th>Description</th>
        <th>Categories</th>
    </thead>
    <tbody>
        <tr><td rowspan="7"><b>Standard</b></td><td rowspan="7">This template assists in creating an application that adheres to the standard security protocols, such as Ws-Federation, OPENID or SAML2</td><td>User Agent Based application (SPA)</td></tr>
        <tr><td>Machine-to-Machine(M2M) applications</td></tr>
        <tr><td>Web application executed on server</td></tr>
        <tr><td>Desktop or mobile application</td></tr>
        <tr><td>A WS-Federation relying party</td></tr>
        <tr><td>A Service Provider (SP)</td></tr>
        <tr><td>Device - An IoT application</td></tr>
        <tr><td rowspan="3"><b>FAPI2.0</b></td><td rowspan="3">This template assists in creating an application used in the Financial Domain. This application must be highly secure and must adhere to a set of security practices.</td><td>Highly secure Web Application</td></tr>
        <tr><td>Grant Management</td></tr>
        <tr><td>External Device Authentication</td></tr>
    </tbody>
</table>

To manage the clients, navigate to the `Clients` menu.

![Clients](./images/clients.png)

You can see the configuration of the client, by clicking on his identifier displayed in the first column of the table.

The view of the client displays six tab elements :

* **Details** : Display the details of the client; depending on the nature of the client (Machine or website), the parameters displayed are different. For example, if the client is a Single Page Application then the Redirect Urls are displayed, otherwise if the client is a Console Application/Machine then the Redirect Urls are not displayed.

* **Client Scopes** Display the list of [Scopes](../glossary) that the client have access to. There are two types of scopes, [Identity Scope](../glossary) and [Api Scope](../glossary), Identity Scope grants access to the client to certain claims of the authenticated user for example Email. Api Scope grants access to certain [Api Resources](../glossary) for example Read action on the Clients REST.API.

* **Keys** : Some Client Authentication Methods require Client Secret signed by a key such as private_key_jwt. The Identity Provider store the public keys or use the Json Web Key Url exposed by the Client, to verify the signature of the Client Secret. 

* **Credentials**: Choose the authentication method.

* **Roles** : One or more user's roles can be assigned to a client for example administrator. The role can be assigned to a group, and the group can be assigned to one or more users. The role of the client will be present in the claims `role` of the authenticated user, when the token is received by the client, he can check the permissions.

* **Advanced**: Display all the possible parameters of a client. 

![Client view](./images/clientview.png)

## Scopes

The [Scope](../glossary) provide a way to limit the amount of access that is granted to an access token.

There are two types of scopes

| Name                    | Description |
| ----------------------- | ----------- |
| OpenId / Identity Scope | OPENID/Identity scopes are used by a client during authentication to authorize access to a user's details, like name and picture |
| API Scope               | Limit the access to REST.API / Resources |

To manage the scopes, navigate to the `Scopes` menu.

### Identity Scope

An [Identity Scope](../glossary) contains one or more [Mapping Rules](../glossary), they are used to fetch user informations and transform them into a list of claims which will be part of the Identity Token.

There are two types of mapping rule

| Name           | Description                                                                                              |
| -------------- | -------------------------------------------------------------------------------------------------------- |
| User Attribute | Transform a user's claim into an output claim; claims are not static and can take any form               |
| User Property  | Transform a property of a user into an output claim; properties are static and defined by SimpleIdServer |

### API Scope

An [API scope](../glossary) represents a permission on one or more API resources, for example `read` on the client named `ClientsApi`.

The `aud` claim is populated with the revelant API resources. This claim is utilized by the API during the authorization process.