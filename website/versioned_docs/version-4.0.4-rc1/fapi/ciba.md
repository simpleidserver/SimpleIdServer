# Client Initiated Backchannel Authentication (CIBA) Profile

Client Initiated Backchannel Authentication (CIBA) is an extension to the OAuth 2.0 framework that allows for authentication without user interaction. 
Traditionally, in OAuth 2.0, the user is prompted to provide their credentials directly to the authorization server during the authorization process. 
However, in scenarios where user interaction is not feasible or desired, CIBA provides an alternative mechanism.

The Client Initiated Backchannel Authentication (CIBA) process involves several steps. Here is a high-level overview of the typical flow:
1. Client Initiation: The client application sends an authentication request to the authorization server on behalf of the user. This request includes information such as the client identifier, requested scopes, and the desired authentication method.
2. Authentication Request Processing: The authorization server receives the authentication request from the client and validates the request parameters. It determines the appropriate authentication method based on the client's request and configuration.
3. User Authentication: The authorization server initiates the user authentication process. This can involve presenting a user interface, redirecting the user to an external authentication device or service, or using any other designated authentication method.
4. User Consent: If required, the authorization server obtains the user's consent for the requested scopes and permissions. This step ensures that the user explicitly approves the client's access to their protected resources.
5. Authentication Result Notification: Once the user is successfully authenticated, the authorization server sends a result notification back to the client. This notification typically includes an authentication reference (e.g., an authentication request ID) and any other relevant details.
6. Client Polling: The client periodically polls the authorization server's token endpoint, using the authentication reference received in the previous step. The purpose is to check for the completion of user authentication and retrieve the necessary tokens.
7. Token Issuance: Once the authorization server determines that the user authentication is complete, it issues the appropriate tokens, such as an access token or an authorization code, to the client.
8. Accessing Protected Resources: The client can then use the issued tokens to access protected resources on behalf of the user. It includes the tokens in API requests as per the OAuth 2.0 protocol to authenticate and authorize the client's access.

## Api calls

Two endpoints are utilized for Client Initiated Backchannel Authentication (CIBA). The bc-authorize endpoint is responsible for creating the authentication request, while the client polls the token endpoint to obtain an access token.

The first step involves constructing an authentication request. If an authentication device is configured, a notification will be dispatched to the user.

*Request*

```
HTTP POST : https://<domainUri>/master/bc-authorize

Content-Type: x-www-form-urlencoded

client_id=your_client_id
&client_secret=your_client_secret
&scope=openid profile
&login_hint=user
&user_code=password
```

*Response*

``` 
{
    "auth_req_id": "o7Ui5ztCxdwmeJN8pvSl4DIVFYbuy9EnOPXQZ0qK6WBRTckhj1HGAsa3rgML2f",
    "expires_in": 120,
    "interval": 5
}
```

Get an access token 

```
HTTP POST : https://<domainUri>/master/bc-authorize

Content-Type: x-www-form-urlencoded

client_id=your_client_id
&client_secret=your_client_secret
&grant_type=urn:openid:params:grant-type:ciba
&auth_req_id=auth_req_id
```