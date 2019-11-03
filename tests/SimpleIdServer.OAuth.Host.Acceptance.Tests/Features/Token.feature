Feature: Token
	Get access token

Scenario: Use client_credentials grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| scope			| scope1												|
	| grant_type	| client_credentials									|

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use password grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| username		| administrator											|
	| password		| password												|
	| grant_type	| password												|

	And extract JSON from body
	
	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use authorization_code grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And add user consent : user='administrator', scope='scope1', clientId='f3d35cce-de69-45bf-958c-4a8796f8ed37'

	And execute HTTP GET request 'http://localhost/authorization'
	| Key			| Value													|
	| response_type | code													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| state			| state													|
	| redirect_uri  | http://localhost:8080									|
	| response_mode | query													|
	
	And extract parameter 'code' from redirect url

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| grant_type	| authorization_code									|
	| code			| $code$												|
	| redirect_uri  | http://localhost:8080									|

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Use refresh_token grant type to get an access token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| scope			| scope1												|
	| grant_type	| client_credentials									|
	
	And extract JSON from body
	And extract parameter 'refresh_token' from JSON body
	
	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| refresh_token	| $refresh_token$										|
	| grant_type	| refresh_token											|

	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'

Scenario: Revoke refresh_token
	When add JSON web key to Authorization Server and store into 'jwks'
	| Type | Kid | AlgName |
	| SIG  | 1   | RS256   |

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value													|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	| scope			| scope1												|
	| grant_type	| client_credentials									|
	
	And extract JSON from body
	And extract parameter 'refresh_token' from JSON body	
	
	And execute HTTP POST request 'http://localhost/token/revoke'
	| Key			| Value													|
	| token			| $refresh_token$										|
	| client_id		| f3d35cce-de69-45bf-958c-4a8796f8ed37					|
	| client_secret | BankCvSecret											|
	
	Then HTTP status code equals to '200'

Scenario: Use authorization_code grant type to get an access token (PKCE)
	When execute HTTP POST JSON request 'http://localhost/register'
	| Key							| Value						|
	| token_endpoint_auth_method	| pkce						|
	| response_types				| [code]					|
	| grant_types					| [authorization_code]		|
	| scope							| scope1					|
	| redirect_uris					| [http://localhost:8080]	|	

	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='scope1', clientId='$client_id$'
	
	And execute HTTP GET request 'http://localhost/authorization'
	| Key					| Value											|
	| response_type			| code											|
	| client_id				| $client_id$									|
	| state					| state											|
	| scope					| scope1										|
	| code_challenge		| VpTQii5T_8rgwxA-Wtb2B2q9lg6x-KVldwQLwQKPcCs	|
	| code_challenge_method	| S256											|	
	
	And extract parameter 'code' from redirect url	

	And execute HTTP POST request 'http://localhost/token'
	| Key			| Value						|
	| client_id		| $client_id$				|
	| client_secret | BankCvSecret				|
	| grant_type	| authorization_code		|
	| code			| $code$					|
	| code_verifier | code					|
	| redirect_uri  | http://localhost:8080		|
	
	And extract JSON from body

	Then HTTP status code equals to '200'
	Then JSON exists 'access_token'
	Then JSON exists 'refresh_token'
	Then JSON 'token_type'='Bearer'