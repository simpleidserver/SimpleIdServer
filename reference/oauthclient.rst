OAuthClient
===========

``ClientId``
    Client identifier.

``Secrets``
    One or more client secrets.

``ClientNames``
    One or more human readable client name.

``LogoUris``
    One or more URL that references a logo for the client.

``ClientUris``
	One or more URL of a web page providing information about the client.
	
``PolicyUris``
    One or more URL that points to a human-readable policy document for the client.
	
``TosUris``
    One or more URL that points to a human-readable terms of service document for the client.
	
``TokenSignedResponseAlg``
    Cryptographic algorithm used to secure the JWS access token.
	
``TokenEncryptedResponseAlg``
    Cryptographic algorithm used to encrypt the JWS access token.
	
``TokenEncryptedResponseEnc``
    Content encryption algorithm used perform authenticated encryption on the JWS access token.
	
``TokenEndpointAuthMethod``
    Requested authentication method for the token endpoint. The possible values are::
	
	- client_secret_post
	
	- client_secret_basic
	
	- private_key_jwt
	
	- client_secret_jwt

``GrantTypes``
	Array of OAUTH2.0 grant type strings that the client can use at the token endpoint. The possible values are :
	
	- authorization_code
	
	- implicit
	
	- password
	
	- client_credentials
	
	- refresh_token	

``ResponseTypes``
	Array of the OAUTH2.0 response type strings that the client can use at the authorization endpoint. The possible values are :
	
	- code
	
	- token
	
``AllowedScopes``
	Scope values that the client can use when requesting access tokens.  

``RedirectionUrls``
	Array of redirection URIS for use in redirect-based flows.
	
``JwksUri``
	URI string referencing the client's JSON Web Key (JWK) Set document, which contains the client's public keys.
	
``JsonWebKeys``
	Client's JSON Web Key Set document value, which contains the client's public keys. 
	
``TokenExpirationTimeInSeconds``
	Token expiration time in seconds.
	
``RefreshTokenExpirationTimeInSeconds``
	Refresh token expiration time in seconds. 
	
``PreferredTokenProfile``
	Preferred token profile, possible values are : **bearer** or **mac**.
	
``Contacts``
	Array of strings representing was to contact people responsible for the client, typically email addresses.
	
``SoftwareId``
	A unique identifier assigned by the client developer or software publisher used by registration endpoints to identify the client software to be dynamically registered.
	
``SoftwareVersion``
	A version identifier string for the client software identified by **software_id**.