Feature: UserInfoErrors
	Check the errors returned by the UserInfo endpoint

Scenario: Error is returned when the token is missing
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key | Value |

	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: Error is returned when token is incorrect
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value				|
	| Authorization		| Bearer tst tst	|

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: Error is returned when token is not well formatted
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value				|
	| Authorization		| Bearer tst		|

	And extract JSON from body

	Then JSON 'error'='invalid_token'
	Then JSON 'error_description'='bad token'

Scenario: Error is returned when the user is unknown
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                          | Value             |
	| redirect_uris                | [https://web.com] |
	| scope                        | email             |	

	And extract JSON from body
	And extract parameter 'client_id' from JSON body 

	And add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | ES256   |
	
	And use '1' JWK from 'jwks' to build JWS and store into 'accesstoken'
	| Key | Value	|
	| sub | unknown |
	| aud | aud		|

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|

	Then HTTP status code equals to '401'

Scenario: Error is returned when there is not valid audience in the token
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key			| Value				|
	| redirect_uris | [https://web.com] |
	| scope			| email				|
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body

	And add JSON web key to Authorization Server and store into 'jwks'
	| Type	| Kid	| AlgName	|
	| SIG	| 1		| ES256		|
	
	And use '1' JWK from 'jwks' to build JWS and store into 'accesstoken'
	| Key | Value			|
	| sub | administrator	|
	| aud | aud				|	

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='invalid audiences'	

Scenario: Error is returned when no consent has been accepted	
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key			| Value				|
	| redirect_uris | [https://web.com] |
	| scope			| email				|
	
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

	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $accesstoken$	|

	And extract JSON from body

	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='no consent has been accepted'