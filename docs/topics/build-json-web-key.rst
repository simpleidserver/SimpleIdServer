Build JSON Web Key
==================

``SimpleIdServer.Jwt`` library offers builders to help developers to build JSON Web Keys (JWK). Two types of JSON Web Key can be generated.

JSON Web Key Signature (JWKS) is the first type. It is used by the authorization server and relying parties and is needed in different workflows:

- Authorization server uses the JWKS private key to generate access or identity token.

- Authorization server uses the public keys of a relying party to check the signature of ``request`` parameter.

- Relying party use the public keys exposed by the authorization server to check the token's signature.

Only the public keys must be exposed because they are useful for the verification process. The private key must be kept secret and inaccessible.

A JSON Web Key Signature (JWKS) can be built like this::

	using (var rsa = RSA.Create())
	{
		var sigJsonWebKey = new JsonWebKeyBuilder().NewSign("1", new[]
		{
			KeyOperations.Sign,
			KeyOperations.Verify
		}).SetAlg(rsa, "RS256").Build();
	}
	
**NewSign** function accepts two parameters :

1) Key identifier.

2) Array of operations, it identifies the operation(s) for which the key is intended to be used. The parameter should be set to ``Sign`` and ``Verify``.

``SetAlg`` function is used to set the algorithm that will be used during the signature generation process. It contains two parameters :

1) Key, either RSA or Elliptic Curve key can be passed.

2) Algorithm name.

Possible values are listed below :

===========  ===============
   Key     	 Algorithm Name
-----------  ---------------
ECDsaCng  	 ES256
ECDsaCng	 ES384
ECDsaCng	 ES512
HMACSHA256	 HS256
HMACSHA384	 HS384
HMACSHA512	 HS512
RSA			 RS256
RSA			 RS384
RSA			 RS512
===========  ===============

JSON Web Key Encryption (JWKSE) is the second type, it is used by the authorization server and relying parties and is needed in different workflows:

- Authorization server uses the public key of a relying party to encrypt the JSON Web Signature Token (JWS) into JSON Web Encryption Token (JWE).

- Relying party uses his private key to decrypt the JSON Web Encryption Token (JWE) into JSON Web Signature Token (JWS).

- Relying party uses the public key of the authorization server to encrypt the ``request`` parameter.

- Authorization server uses his private key to decrypt the ``request`` parameter.

Only the public keys must be exposed because they are useful for the decryption process. The private key must be kept secret and inaccessible

A JSON Web Key Encryption (JWKSE) can be built like this::

	using (var rsa = RSA.Create())
	{
		var sigJsonWebKey = new JsonWebKeyBuilder().NewEnc("1", new[]
		{
			KeyOperations.Encrypt,
			KeyOperations.Decrypt
		}).SetAlg(rsa, "RSA-OAEP").Build();
	}

Supported algorithms are listed below :

===========  ===============
   Key     	 Algorithm Name
-----------  ---------------
RSA  	 	 RSA-OAEP
RSA	 		 RSA1_5
RSA			 RSA-OAEP-256
===========  ===============
