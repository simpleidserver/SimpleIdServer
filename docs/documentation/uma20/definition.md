# Definition

The User Management Access (UMA) server version 2.0 is an OAUTH2.0 authorization server with a new `uma-ticket` grant-type and some additional endpoints (policy endpoint and resource endpoint). 
Therefore, it can be used to obtain OAUTH2.0 access and refresh tokens.

The UMA2.0 server offers to the end-user a way to manage access to his resources. For example, a user can decide who can access his profile information. The protected resources must be registered in the UMA2.0 server and their identifiers should be stored by the API.

The client who tries to access a protected resource must provide a valid access token which can be obtained with the `uma-ticket` grant-type.
The following schema shows how to access a protected resource

![Schema](images/uma-2.png)

1. The client tries to access to the protected resource without valid access token. An error is returned with the resource id and the UMA server url.
2. The client uses the resource id, the access / identity token and the scopes (read, write, etc...) to get the access token. A token is returned if the authorization policy assigned to the resource is statisfied. An authorization policy can be satisfied via different ways :
    1. If the client_id obtained via the access token matches the excepted one.
    2. If the claims extracted from the identity token match the excepted values.
    2. If the scopes requested by the client are authorized.
3. The client uses the access token to get the protected resource.
4. The access token is checked against the UMA server.
5. The protected resource is returned.