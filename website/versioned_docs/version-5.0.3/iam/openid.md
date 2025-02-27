# OPENID

The SimpleIdServer's [Identity Provider](../glossary) can act as an OPENID server.

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
| urn:ietf:params:oauth:grant-type:token-exchange | [https://datatracker.ietf.org/doc/html/rfc8693](https://datatracker.ietf.org/doc/html/rfc8693) |

## Supported authentication methods

The following client authentication methods are supported.

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

The [Client](../glossary) is the application that is attempting to act on behalf of the user or access the user's resources.

In SimpleIdServer, clients are categorized into [Categories](../glossary), and each category belongs to a [Template](../glossary).

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
        <tr><td><b>Credentials issuer</b></td><td></td><td></td></tr>
    </tbody>
</table>

To manage the clients, navigate to the `Clients` menu.

![Clients](./images/clients.png)

You can view the configuration of the client by clicking on its identifier displayed in the first column of the table.

The client view displays six tab elements:

* **Details** : Display the details of the client; depending on the nature of the client (machine or website), the displayed parameters vary. For instance, if the client is a Single Page Application, the Redirect URLs are displayed; otherwise, if the client is a Console Application/Machine, the Redirect URLs are not displayed.

* **Client Scopes** Display the list of [Scopes](../glossary) to which the client has access. There are two types of scopes : [Identity Scope](../glossary) and [Api Scope](../glossary).Identity Scopes grant the client access to specific claims of the authenticated user, such as Email. API Scopes grant the client access to certain [Api Resources](../glossary), for example, the Read action on the Clients REST API.

* **Keys** : Some client authentication methods require a client secret signed by a key, such as private_key_jwt. The Identity Provider stores the public keys or uses the Json Web Key URL exposed by the client to verify the signature of the client secret.

* **Credentials**: Select the authentication method.

* **Roles** : One or more user roles can be assigned to a client, for example, 'administrator.' The role can also be assigned to a group, and that group can, in turn, be assigned to one or more users. The client's role will be included in the `role` claims of the authenticated user. When the client receives the token, it can check the permissions.

* **Advanced**: Display all the available parameters of a client.

![Client view](./images/clientview.png)

## Scopes

The [Scope](../glossary) provides a means to restrict the level of access granted to an access token.

There are two types of scopes:

| Name                    | Description |
| ----------------------- | ----------- |
| OpenId / Identity Scope | OPENID/Identity scopes are used by a client during authentication to authorize access to a user's details, such as name and picture. |
| API Scope               | Restrict access to REST API/resources. |

To manage the scopes, navigate to the `Scopes` menu.

### Identity Scope

An [Identity Scope](../glossary) contains one or more [Mapping Rules](../glossary). These rules are used to retrieve user information and convert it into a list of claims that will be included in the Identity Token.

There are two types of mapping rules:

| Name           | Description                                                                                              |
| -------------- | -------------------------------------------------------------------------------------------------------- |
| User Attribute | Transform a user's claim into an output claim; claims are not static and can take any form               |
| User Property  | Transform a user's property into an output claim; properties are static and defined by SimpleIdServer |

### API Scope

An [API scope](../glossary) represents a permission for one or more API resources, such as `read` for the client named `ClientsApi`.

The `aud` claim is populated with the relevant API resources. This claim is used by the API during the authorization process.