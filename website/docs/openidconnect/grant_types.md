# Supported grant types

## Authorization Code

The Authorization Code grant type is an OAuth 2.0 flow that allows a client application to request authorization to access protected resources on behalf of a user. 
It is commonly used when the client application is a web application running on a server.

Here's a step-by-step explanation of how the Authorization Code grant type works:

1. The client application redirects the user to an authorization server with the necessary parameters. These parameters typically include the client ID, the requested scope of access, a redirect URI, and a response type set to "code."
2. The user is presented with a login screen by the authorization server and enters their credentials.
3. Once authenticated, the authorization server presents the user with a consent screen, requesting their permission to grant the requested access rights to the client application.
4. If the user grants permission, the authorization server redirects the user back to the client application's redirect URI, along with an authorization code. The redirect URI is provided as part of the initial request in step 1.
5. The client application receives the authorization code from the redirect URI.
6. The client application makes a back-end request to the authorization server, providing the authorization code, the client secret (if applicable), and the redirect URI.
7. The authorization server verifies the provided information, including validating the authorization code and verifying the client's identity. If the verification is successful, the authorization server generates an access token and, optionally, a refresh token.
8. The authorization server responds to the client application with an access token and, if issued, a refresh token. The access token is a short-lived token that grants access to protected resources. The refresh token is a long-lived token that can be used to obtain a new access token when the current one expires.
9. The client application can then use the access token to make authorized requests to the protected resources on behalf of the user. The access token is typically included in the Authorization header of API requests.
10. If the access token expires, the client application can use the refresh token to request a new access token from the authorization server without involving the user. This helps to maintain a seamless user experience.

### API calls

The authorization API is needed to get an authorization code, and the token API is needed to get an access token.

Example of authorization request :

```
GET /authorization?
   response_type=code&
   &client_id=your_client_id
   &redirect_uri=https://your-app.com/callback
   &scope=openid profile
   &state=1234567890
```

Example of authorization response in case the user is authenticated :

```
GET https://your-app.com/callback?
   state=1234567890
   &code=1e32076f-b27f-4e03-9b34-e19dcb5efe11
   &token_type=Bearer
   &session_state=AVinZKQq4XTdDezZKi0rJFT9Zs2ikPgr7FZTFMyxuQA.9057a6c7-1d3c-42df-83d2-a47c14e288d0
```

Example of token request :

```
POST https://<domainUri>/token

Content-Type: x-www-form-urlencoded

client_id=website
&grant_type=authorization_code
&redirect_uri=http://localhost
&client_secret=password
&code=<authorization code>
```

## Implicit Grant-Type

The Implicit Grant Type is another OAuth 2.0 flow that allows client applications, particularly those running in web browsers, to obtain access tokens for accessing protected resources. 
Unlike the Authorization Code grant type, the Implicit grant type does not involve the exchange of an authorization code.

Here's a step-by-step explanation of how the Implicit grant type works:

1. The client application redirects the user to an authorization server with the necessary parameters. These parameters typically include the client ID, the requested scope of access, a redirect URI, and a response type set to "token" (indicating the use of the Implicit grant type).
2. The user is presented with a login screen by the authorization server and enters their credentials.
3. Once authenticated, the authorization server presents the user with a consent screen, requesting their permission to grant the requested access rights to the client application.
4. If the user grants permission, the authorization server generates an access token.
5. The authorization server redirects the user back to the client application's redirect URI, along with the access token and other optional parameters. The redirect URI is provided as part of the initial request in step 1.
6. The client application receives the access token from the redirect URI.
7. The client application can then use the access token to make authorized requests to the protected resources on behalf of the user. The access token is typically included in the URL fragment (after the "#" character) or as a query parameter in the redirect URI.

### API calls

Only the authorization endpoint is needed.

Example of authorization request:

```
GET /authorization?
   response_type=token&
   &client_id=your_client_id
   &redirect_uri=https://your-app.com/callback
   &scope=openid profile
   &state=1234567890
```

Example of authorization response in case the user is authenticated :

```
GET https://your-app.com/callback
   #state=1234567890
   &access_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IndzRmVkS2lkIiwieDV0IjoiTmtCMC05UlpkTDgtdnVmcVVoOWtDUklsbVlnIiwidHlwIjoiSldUIn0.eyJhdWQiOltdLCJjbGllbnRfaWQiOiJ3ZWJzaXRlIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSJdLCJzdWIiOiJhZG1pbmlzdHJhdG9yIiwiYXV0aF90aW1lIjoxNjg2NTczMzI0LjAsImV4cCI6MTY4NjU4MjM5NywiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTAwMS9tYXN0ZXIiLCJpYXQiOjE2ODY1ODA1OTcsIm5iZiI6MTY4NjU4MDU5N30.D9lIADb8M9WNkdGG6vERQBscQyFeNXpVOXLGOxHjyc3kKthDhvM8JGYx6Q3Lnkhs_zvySYS72bLV8jpDUIA6tctvIK7UpsbckumoKVeoCpqvx44U3_Rj5kkm5ICuzSggJhR6iQalYYnML4nQk05OhiVKUAw0Sn--2wXRuo7R6PexiR60KHbhP11NtDve8QdXaoj0IbX-vqEVqlyxcYit0DSYzTWiQ0vNnYmHIExJj_4SLbvURjiWuydqEl58IFrbyFyHKXiVzG7odREps9YUDqxbclxUlQI4igcKTuRZG2KOriBBmt-GGsN1KX-jc21G0gLrg-P0x8MwimrZ-nb2qA
   &token_type=Bearer
   &session_state=oJK5q8N1SnWeXGsLxRdSmpZiIKSgnJn1D7enzyA6_xE.c5ea6702-2d7a-4f2f-8687-c0618545d973
```

## Client Credentials

The Client Credentials grant type is an OAuth 2.0 flow that allows a client application to authenticate and obtain access tokens directly from an authorization server, without involving a user. This flow is typically used for server-to-server communication or when the client application is acting on its own behalf rather than on behalf of a specific user.

Here's a step-by-step explanation of how the Client Credentials grant type works:

1. The client application authenticates itself with the authorization server by sending a request that includes its client ID and client secret. These credentials are typically obtained during the client application registration with the authorization server.
2. The authorization server validates the client credentials and verifies the client's identity.
3. If the client credentials are valid, the authorization server generates an access token specific to the client application.
4. The authorization server responds to the client application with the access token.
5. The client application can then use the access token to make authorized requests to protected resources, typically by including the access token in the Authorization header of API requests.

### API calls

Only the token endpoint is needed.

Example of token request :

```
POST https://<domainUri>/token

Content-Type: x-www-form-urlencoded

client_id=device
&grant_type=client_credentials
&client_secret=password
&scope=networks
```

## Resource Owner Password Credentials

The Resource Owner Password Credentials grant type is an OAuth 2.0 flow that allows a client application to obtain access tokens by directly exchanging the resource owner's (user's) credentials with the authorization server. 
This flow is typically used when the client application is highly trusted and has the ability to securely collect and store the resource owner's credentials, such as in the case of native or mobile applications.

Here's a step-by-step explanation of how the Resource Owner Password Credentials grant type works:

1. The client application collects the resource owner's (user's) username and password.
2. The client application sends a request to the authorization server, including the resource owner's credentials, the client ID, the client secret (if applicable), and the requested scope of access.
3. The authorization server validates the resource owner's credentials and verifies the client's identity.
4. If the credentials are valid, the authorization server generates an access token specific to the resource owner.
5. The authorization server responds to the client application with the access token.
6. The client application can then use the access token to make authorized requests to protected resources, typically by including the access token in the Authorization header of API requests.

### API calls

Only the token endpoint is needed.

Example of token request :

```
POST https://<domainUri>/token

Content-Type: x-www-form-urlencoded

client_id=userDevice
&grant_type=password
&client_secret=password
&scope=openid profile
&username=<login>
&password=<password>
```

## Device flow

The grant type "urn:ietf:params:oauth:grant-type:device_code" is known as the Device Authorization Grant Type, also referred to as the Device Flow. 
It is an OAuth 2.0 extension designed to enable devices with limited input capabilities, such as smart TVs or IoT devices, to obtain access tokens to access protected resources.

Here's a step-by-step explanation of how the Device Authorization Grant Type works:

1. The device sends a request to the authorization server to initiate the device flow. This request typically includes the client ID, the requested scope of access, and the grant type set to "urn:ietf:params:oauth:grant-type:device_code".
2. The authorization server generates a device code and provides it in the response to the device.
3. The device presents the device code to the end-user through its display capabilities or another suitable means.
4. The end-user uses a separate device, such as a smartphone or computer, to authenticate with the authorization server.
5. The end-user enters the device code provided by the device into the authorization server's user interface.
6. The authorization server verifies the device code and prompts the end-user to authenticate and authorize the device to access the requested resources.
7. If the end-user provides consent, the authorization server generates an access token and returns it to the device.
8. The device can then use the access token to make authorized requests to protected resources on behalf of the end-user.

### API calls

The browser-less device initiates the flow by sending the following request:

```
POST https://<domainUri>/token

Content-Type: x-www-form-urlencoded

client_id=device
&scope=openid profile
```

API responds with :

``` 
{
    "device_code": "bd2efb74-4213-4ded-a719-0ec9ecf48e4f",
    "user_code": "7096",
    "verification_uri": "https://localhost:5001/master/Device",
    "verification_uri_complete": "https://localhost:5001/master/Device?userCode=7096",
    "expires_in": 600,
    "interval": 5
}
```

Response contains the following parameters :

* `device_code` : A unique identifier for the device code that the client application will use to poll the authorization server and obtain an access token.
* `user_code` :  A short alphanumeric code that the user needs to enter on a separate device to complete the authorization process.
* `verification_uri` :  The URL or URI where the user should visit on a separate device to enter the user_code.
* `verification_uri_complete` : The complete URL that the user should visit, including any additional parameters that may be required.
* `expires_in` : The duration of time in seconds for which the device_code and user_code are valid. After this time has elapsed, the client application should not use the codes for further authorization attempts.
* `interval` : he number of seconds that the client application should wait before polling the token endpoint to check if the user has completed the authorization process. This interval is typically used to avoid excessive requests to the server.

The client application will poll the token endpoint until the user has completed the authorization process.

This request is sent to the token endpoint :

```
POST https://<domainUri>/token

Content-Type: x-www-form-urlencoded

client_id=device
&grant_type=urn:ietf:params:oauth:grant-type:device_code
&client_secret=password
&device_code=<device code>
```

After the user successfully enters the code, an access token along with an id_token will be returned to the device :

```
{
    "expires_in": 1800,
    "scope": "openid profile",
    "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IndzRmVkS2lkIiwieDV0IjoiTmtCMC05UlpkTDgtdnVmcVVoOWtDUklsbVlnIiwidHlwIjoiSldUIn0.eyJhdWQiOm51bGwsImNsaWVudF9pZCI6Im90aGVyRGV2aWNlIiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSJdLCJzdWIiOiJhZG1pbmlzdHJhdG9yIiwiZXhwIjoxNjg2NTk3NzA1LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo1MDAxL21hc3RlciIsImlhdCI6MTY4NjU5NTkwNSwibmJmIjoxNjg2NTk1OTA1fQ.wYBMh81XMSwmrvV2e0Sfp4JYM8pyhwSHUqP9AafD0TakayRQw3p0gBjWszu0B1GDFPN-6InxhjIRq_OQ5DVNUhMmplpSuTjjCn34JushUa6kcTArzUZOwjeiWTUg0Pe0gx3hXDpWY3kdxsikSmW3jlCLgGpI7L-6FN_wqtajx2uyRxinHb2rrjrR4vDtISN-I-l65c4h4exSig6W15Db3L4zmwUnC8OecIPDsfA56Lcs2n3rNeqeZw6_L7JXUUIGOogZoZJGYS-0HQl-TuW-bo0pohH7RjnE7tPPEwE12h1guy7pyqd3SN_XmN5HkQRfy2qhfM2f9roiZFolNzJFog",
    "id_token": "eyJhbGciOiJFUzI1NiIsImtpZCI6ImVjZHNhLTEiLCJ0eXAiOiJKV1QifQ.eyJhdWQiOlsib3RoZXJEZXZpY2UiLCJodHRwczovL2xvY2FsaG9zdDo1MDAxL21hc3RlciJdLCJhenAiOiJvdGhlckRldmljZSIsImF0X2hhc2giOiJ5T3hCTHZ3aXR4WlE0Q2xldEEzZXFBIiwiYW1yIjpbInB3ZCJdLCJhY3IiOiJzaWQtbG9hZC0wMSIsInN1YiI6ImFkbWluaXN0cmF0b3IiLCJleHAiOjE2ODY1OTc3MDUsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjUwMDEvbWFzdGVyIiwiaWF0IjoxNjg2NTk1OTA1LCJuYmYiOjE2ODY1OTU5MDV9.O9fGKXQCIr4CYOwf5TQ6RXe7LBsUHBn3oU7vnFjIpdYtdYi9YLh1DTct_1Rae2atHVmUkB0sNUl7opy3jLv-jw",
    "refresh_token": "5e63301c-e320-49aa-a9cf-959e26bfd818",
    "token_type": "Bearer"
}
```