OpenIDClient
============

OPENID client inherits properties from OAUTH2.0 client.

``ApplicationType``
    Kind of the application. The default, if omitted, is **web**. The defined values are **native** or **web**. 

``IdTokenSignedResponseAlg``
    Cryptographic algorithm used to secure the JWS identity token. 

``IdTokenEncryptedResponseAlg``
	Cryptographic algorithm used to encrypt the JWS identity token.

``IdTokenEncryptedResponseEnc``
    Content encryption algorithm used perform authenticated encryption on the JWS identity token.

``UserInfoSignedResponseAlg``
	Required for signing UserInfo responses.
	
``UserInfoEncryptedResponseAlg``
    Required for encrypting the identity token issued to this client.
	
``UserInfoEncryptedResponseEnc``
    Required for encrypting the identity token issued to this client.
	
``RequestObjectSigningAlg``
    Must be used for signing Request Objects sent to the OpenID provider.
	
``RequestObjectEncryptionAlg``
    JWE alg algorithm the relying party is declaring that it may use for encrypting Request Objects sent to the OpenID provider.
	
``RequestObjectEncryptionEnc``
    JWE enc algorithm the relying party is declaring that it may use for encrypting request objects sent to the OpenID provider.
	
``SubjectType``
    subject_type requested for responses to this client. Possible values are **pairwise** or **public**

``DefaultMaxAge``
	Default Maximum Authentication Age.
	
``DefaultAcrValues``
	ADefault requested Authentication Context Class Reference values.
	
``RequireAuthTime``
	Boolean value specifying whether the auth_time claim in the identity token is required.
	
``SectorIdentifierUri``
	URI using the HTTPS scheme to be used in calculating Pseudonymous Identifiers by the OpenID provider.
	
``PairWiseIdentifierSalt``
	SALT used to calculate the pairwise.