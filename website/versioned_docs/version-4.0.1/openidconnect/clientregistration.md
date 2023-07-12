# Dynamic client registration API

The Dynamic Client Registration API is an optional feature provided by the OAuth 2.0 framework that allows client applications to dynamically register themselves with an authorization server. 
This API enables client applications to programmatically provide information about themselves to the server, eliminating the need for manual registration and configuration.

Here's an overview of how the Dynamic Client Registration API works:

1. Registration Request: The client application sends a registration request to the authorization server's registration endpoint, typically using the HTTP POST method. The request contains a set of parameters that provide information about the client application, such as its name, redirect URIs, supported grant types, client type, etc.
2. Registration Response : The authorization server processes the registration request and responds with a registration response. This response includes important information for the client application, such as a unique client identifier (client_id) and a client secret (client_secret). The response may also contain other metadata or configuration information specific to the authorization server's implementation.

By using the Dynamic Client Registration API, client applications can automate the registration process, reduce manual configuration efforts, and ensure compatibility with the authorization server's requirements. 
It provides flexibility for client applications to dynamically adjust their registration details as needed, enhancing the overall efficiency and scalability of the OAuth 2.0 ecosystem. 

### API Calls

Obtain an access token that is valid for the registration scope.

```
POST: https://<domainUri>/token

Content-Type: x-www-form-urlencoded

client_id=registerClient
&grant_type=client_credentials
&client_secret=password
&scope=register
```

Send the following request to create a minimalist client :

**Request**

```
POST: https://<domainUri>/register

Authorization: Bearer <access token>
Content-Type: application/json

{
    "grant_types": ["client_credentials"],
    "scope": "openid profile",
    "redirect_uris": ["https://tmp.com"]
}
```

**Response**

```
{
    "client_id": "d371c91b-f48c-4944-ba2e-da3d05d288d3",
    "client_secret": "a2c7b54f-7325-4429-923b-0ba7eb149dc5",
    "registration_access_token": "3a0e9284-0931-49e7-963e-11958e3939e3",
    "grant_types": [
        "client_credentials"
    ],
    "redirect_uris": [
        "https://tmp.com"
    ],
    "token_endpoint_auth_method": "client_secret_post",
    "response_types": [
        "code"
    ],
    "contacts": [],
    "update_datetime": "2023-06-12T19:24:24.0727511Z",
    "create_datetime": "2023-06-12T19:24:24.0726061Z",
    "client_id_issued_at": 1686597864,
    "token_expiration_time_seconds": 1800,
    "refresh_token_expiration_time_seconds": 1800,
    "scope": "profile openid",
    "jwks": {
        "keys": []
    },
    "token_signed_response_alg": "RS256",
    "token_encrypted_response_alg": "RSA1_5",
    "token_encrypted_response_enc": "A128CBC-HS256",
    "post_logout_redirect_uris": [],
    "preferred_token_profile": "Bearer",
    "subject_type": "public",
    "id_token_signed_response_alg": "RS256",
    "backchannel_user_code_parameter": false,
    "frontchannel_logout_session_required": false,
    "backchannel_logout_session_required": false,
    "tls_client_certificate_bound_access_token": false,
    "application_type": "web",
    "require_auth_time": false,
    "authorization_signed_response_alg": "RS256",
    "authorization_data_types": [],
    "is_consent_disabled": false,
    "default_acr_values": [],
    "registration_client_uri": "https://localhost:5001/d371c91b-f48c-4944-ba2e-da3d05d288d3"
}
```