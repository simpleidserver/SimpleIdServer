Feature: AuthorizationErrors
	Check the errors returned by the authorization endpoint

Scenario: Error is returned when response_type parameter is missing
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key			| Value													|

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter response_type'

Scenario: Error is returned when response_type parameter is invalid
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value   |
	| response_type | invalid |

	And extract JSON from body

	Then JSON 'error'='unsupported_response_type'
	Then JSON 'error_description'='missing response types invalid'

Scenario: Error is returned when client_id parameter is missing
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value |
	| response_type | code  |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter client_id'
	
Scenario: Error is returned when client doesn't exist
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value    |
	| response_type | code     |
	| client_id     | clientId |
	| state         | state    |

	And extract JSON from body

	Then JSON 'error'='invalid_client'
	Then JSON 'error_description'='unknown client clientId'

Scenario: Error is returned when redirect_uri is invalid
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                                |
	| response_type | code                                 |
	| client_id     | f3d35cce-de69-45bf-958c-4a8796f8ed37 |
	| state         | state                                |
	| redirect_uri  | invalid                              |

	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='redirect uri invalid is not correct'

Scenario: Error is returned when response_mode is not supported
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                                |
	| response_type | code                                 |
	| client_id     | f3d35cce-de69-45bf-958c-4a8796f8ed37 |
	| state         | state                                |
	| redirect_uri  | http://localhost:8080                |
	| response_mode | invalid                              |
	
	And extract query parameters into JSON

	Then JSON 'error'='unsupported_response_mode'
	Then JSON 'error_description'='response mode invalid is not supported'

Scenario: Error is returned when scope is not supported by the client
	When execute HTTP GET request 'https://localhost:8080/authorization'
	| Key           | Value                                |
	| response_type | code                                 |
	| client_id     | f3d35cce-de69-45bf-958c-4a8796f8ed37 |
	| state         | state                                |
	| redirect_uri  | http://localhost:8080                |
	| scope         | role                                 |
	
	And extract query parameters into JSON

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='scopes role are not supported'

Scenario: Error is returned when the code_challenge parameter is missing
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key							| Value					|
	| redirect_uris					| [http://localhost]	|
	| token_endpoint_auth_method	| pkce					|
	| response_types				| [code]				|
	| grant_types					| [authorization_code]	|
	| scope							| scope1				|
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='scope1', clientId='$client_id$'
	
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key				| Value				|
	| response_type		| code				|
	| client_id			| $client_id$		|
	| state				| state				|
	| scope				| scope1			|
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='missing parameter code_challenge'

Scenario: Error is returned when the code_challenge_method parameter is invalid
	When execute HTTP POST JSON request 'https://localhost:8080/register'
	| Key							| Value					|
	| redirect_uris					| [http://localhost]	|
	| token_endpoint_auth_method	| pkce					|
	| response_types				| [code]				|
	| grant_types					| [authorization_code]	|
	| scope							| scope1				|
	
	And extract JSON from body
	And extract parameter 'client_id' from JSON body	
	And add user consent : user='administrator', scope='scope1', clientId='$client_id$'
	
	And execute HTTP GET request 'https://localhost:8080/authorization'
	| Key					| Value				|
	| response_type			| code				|
	| client_id				| $client_id$		|
	| state					| state				|
	| scope					| scope1			|
	| code_challenge		| code_challenge	|
	| code_challenge_method	| invalid			|
	
	And extract JSON from body

	Then JSON 'error'='invalid_request'
	Then JSON 'error_description'='transform algorithm invalid is not supported'