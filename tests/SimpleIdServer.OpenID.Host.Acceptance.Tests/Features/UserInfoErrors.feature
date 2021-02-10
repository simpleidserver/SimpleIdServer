Feature: UserInfoErrors
	Check the errors returned by the UserInfo endpoint

Scenario: Error is returned when the token is missing (HTTP GET)
	When execute HTTP GET request 'http://localhost/userinfo'
	| Key | Value |

	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: Error is returned when the token is missing (HTTP POST + FORMURLENCODED)
	When execute HTTP POST request 'http://localhost/userinfo'
	| Key | Value |

	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing token'

Scenario: Error is returned when the token is missing (HTTP POST + JSON)
	When execute HTTP POST JSON request 'http://localhost/userinfo'
	| Key | Value |

	And extract JSON from body
	
	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='the content-type is not correct'

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

Scenario: If access token is rejected then userinfo endpoint cannot be accessed
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And build JSON Web Keys, store JWKS into 'jwks' and store the public keys into 'jwks_json'
	| Type | Kid | AlgName |
	| ENC  | 2   | RSA1_5  |

	When execute HTTP POST JSON request 'http://localhost/register'
	| Key                             | Value                         |
	| redirect_uris                   | [https://web.com]             |
	| grant_types                     | [implicit,authorization_code] |
	| response_types                  | [token,id_token,code]         |
	| scope                           | email role                    |
	| subject_type                    | public                        |
	| id_token_signed_response_alg    | RS256                         |
	| id_token_encrypted_response_alg | RSA1_5                        |
	| id_token_encrypted_response_enc | A256CBC-HS512                 |
	| jwks                            | $jwks_json$                   |
	| token_endpoint_auth_method      | client_secret_post            |

	And extract JSON from body
	And extract parameter 'client_id' from JSON body
	And extract parameter 'client_secret' from JSON body
	And add user consent : user='administrator', scope='email role', clientId='$client_id$'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key           | Value               |
	| response_type | id_token token code |
	| client_id     | $client_id$         |
	| state         | state               |
	| response_mode | query               |
	| scope         | openid email role   |
	| redirect_uri  | https://web.com     |
	| ui_locales    | en fr               |
	| nonce         | nonce               |

	And extract 'id_token' from callback
	And extract 'code' from callback

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| grant_type    | authorization_code |
	| code          | $code$             |
	| redirect_uri  | https://web.com    |
	
	And extract JSON from body
	And extract parameter 'access_token' from JSON body

	And execute HTTP POST request 'http://localhost/token'
	| Key           | Value              |
	| client_id     | $client_id$        |
	| client_secret | $client_secret$    |
	| grant_type    | authorization_code |
	| code          | $code$             |
	| redirect_uri  | https://web.com    |
	
	And execute HTTP GET request 'http://localhost/userinfo'
	| Key				| Value					|
	| Authorization		| Bearer $access_token$	|

	And extract JSON from body
	
	Then HTTP status code equals to '400'
	Then JSON 'error'='invalid_token'
	Then JSON 'error_description'='access token has been rejected'