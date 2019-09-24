Feature: UserInfo
	Check the userinfo endpoint

Scenario: Check user information are returned (JSON)
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key			| Value				|
	| redirect_uris | [https://web.com] |
	| scope			| profile			|	
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body

	And add JSON web key to Authorization Server and store into 'jwks'
	| Type	| Kid	| AlgName	|
	| SIG	| 1		| ES256		|	
	
	And use '1' JWK from 'jwks' to build JWS and store into 'accesstoken'
	| Key	| Value				|
	| sub	| administrator		|
	| aud	| $client_id$		|	
	| scope | [openid,profile]	|

	And add user consent : user='administrator', scope='profile', clientId='$client_id$'	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|

	And extract JSON from body

	Then HTTP status code equals to '200'	
	Then HTTP header 'Content-Type' contains 'application/json'
	Then JSON 'name'='Thierry Habart'

Scenario: Check user information are returned (JWS)
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |
	
	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value				|
	| redirect_uris					| [https://web.com] |
	| scope							| profile			|
	| userinfo_signed_response_alg	| ES256				|

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	
	And use '1' JWK from 'jwks' to build JWS and store into 'accesstoken'
	| Key	| Value				|
	| sub	| administrator		|
	| aud	| $client_id$		|	
	| scope | [openid,profile]	|

	And add user consent : user='administrator', scope='profile', clientId='$client_id$'	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|

	And extract string from body
	Then HTTP status code equals to '200'
	Then HTTP header 'Content-Type' contains 'application/jwt'

Scenario: Check user information are returned (JWE)
	When add JSON web key to Authorization Server and store into 'jwks_sig'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |
	
	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| ENC  | 2   | RSA1_5  |
	
	And execute HTTP POST JSON request 'http://localhost/register'
	| Key								| Value				|
	| redirect_uris						| [https://web.com] |
	| scope								| profile			|
	| userinfo_signed_response_alg		| ES256				|
	| userinfo_encrypted_response_alg	| RSA1_5			|
	| userinfo_encrypted_response_enc	| A128CBC-HS256		|
	| jwks								| $jwks_json$		|

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	
	And use '1' JWK from 'jwks_sig' to build JWS and store into 'accesstoken'
	| Key	| Value				|
	| sub	| administrator		|
	| aud	| $client_id$		|	
	| scope | [openid,profile]	|
	
	And add user consent : user='administrator', scope='profile', clientId='$client_id$'		

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|
	
	And extract string from body
	Then HTTP status code equals to '200'
	Then HTTP header 'Content-Type' contains 'application/jwt'

Scenario: Use claims parameter to get user information from UserInfo endpoint
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |	
	
	And execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value				|
	| redirect_uris					| [https://web.com] |
	| scope							| email				|

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	
	And use '1' JWK from 'jwks' to build JWS and store into 'accesstoken'
	| Key		| Value																		|
	| sub		| administrator																|
	| aud		| $client_id$																|	
	| claims	| { userinfo: { name: { essential : true }, email: { essential : true } } }	|	

	And add user consent with claim : user='administrator', scope='email', clientId='$client_id$', claim='name email'	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|

	And extract JSON from body
	Then HTTP status code equals to '200'
	Then HTTP header 'Content-Type' contains 'application/json'
	Then JSON 'name'='Thierry Habart'
	Then JSON 'email'='habarthierry@hotmail.fr'

Scenario: Use offline_access scope to get user information from UserInfo endpoint
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |	

	When execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value									|
	| redirect_uris					| [https://web.com]						|
	| grant_types					| [authorization_code,refresh_token]	|
	| response_types				| [code]								|
	| scope							| offline_access						|
	| subject_type					| public								|
	| token_endpoint_auth_method	| client_secret_post					|

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And extract parameter 'client_secret' from JSON body	
	And add user consent : user='administrator', scope='offline_access', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value								 |
	| response_type | code								 |
	| client_id     | $client_id$						 |
	| state         | state								 |
	| response_mode | query								 |
	| scope         | openid offline_access		         |
	| redirect_uri  | https://web.com					 |
	| ui_locales    | en fr								 |
	
	And extract parameter 'refresh_token' from redirect url

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value				|
	| client_id		| $client_id$		|
	| client_secret | $client_secret$	|
	| grant_type	| refresh_token		|
	| refresh_token | $refresh_token$	|

	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key			| Value					|
	| Authorization	| Bearer $access_token$	|

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then HTTP header 'Content-Type' contains 'application/json'
	Then JSON 'sub'='administrator'